using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TranAnhShop.Models;
using System.IO;
using TranAnhShop.Common;

namespace TranAnhShop.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private TranAnhShopDbContext db = new TranAnhShopDbContext();
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        // GET: Admin/Product
        public ActionResult Index()
        {
            ViewBag.countTrash = db.Product.Where(m => m.Status == 0).Count();
            var list = from p in db.Product
                       join c in db.Category
                       on p.CateID equals c.ID
                       where p.Status != 0 
                       where p.CateID == c.ID
                       orderby p.Created_at descending
                       select new ProductCategory()
                       {
                           ProductID = p.ID,
                           ProductImage = p.Image,
                           ProductName = p.Name,
                           ProductStatus = p.Status,
                           CategoryName = c.Name
                       };
            return View(list.ToList());
        }
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        public ActionResult Trash()
        {
            var list = from p in db.Product
                       join c in db.Category
                       on p.CateID equals c.ID
                       where p.Status == 0
                       where p.CateID == c.ID
                       orderby p.Created_at descending
                       select new ProductCategory()
                       {
                           ProductID = p.ID,
                           ProductImage = p.Image,
                           ProductName = p.Name,
                           ProductStatus = p.Status,
                           CategoryName = c.Name
                       };
            return View(list.ToList());
        }
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            mProduct mProduct = db.Product.Find(id);
            if (mProduct == null)
            {
                Notification.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            return View(mProduct);
        }
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        public ActionResult Create()
        {
            mCategory mCategory = new mCategory();
            ViewBag.ListCat = new SelectList(db.Category.Where(m => m.Status != 0), "ID", "Name", 0);
            //ViewBag.ListCat = new SelectList(db.Category.ToList(), "ID", "Name", 0);
            return View();
        }
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(mProduct mProduct)
        {
            ViewBag.ListCat = new SelectList(db.Category.Where(m => m.Status != 0), "ID", "Name", 0);
            if (ModelState.IsValid)
            {
                mProduct.Price = mProduct.Price ;
                mProduct.ProPrice = mProduct.ProPrice ;

                String strSlug = XString.ToAscii(mProduct.Name);
                mProduct.Slug = strSlug;
                mProduct.Created_at = DateTime.Now;
                mProduct.Created_by = 1;
                mProduct.Updated_at = DateTime.Now;
                mProduct.Updated_by = 1;

                // Upload file
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    mProduct.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Public/images/products/"), filename);
                    file.SaveAs(Strpath);
                }
                db.Product.Add(mProduct);
                db.SaveChanges();
                Notification.set_flash("Thêm mới sản phẩm thành công!", "success");
                return RedirectToAction("Index");
            }
            return View(mProduct);
        }
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        public ActionResult Edit(int? id)
        {
            ViewBag.ListCat = new SelectList(db.Category.ToList(), "ID", "Name", 0);
            mProduct mProduct = db.Product.Find(id);
            if (mProduct == null)
            {
                Notification.set_flash("404!", "warning");
                return RedirectToAction("Index", "Product");
            }
            return View(mProduct);
        }
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(mProduct mProduct)
        {
            ViewBag.ListCat = new SelectList(db.Category.ToList(), "ID", "Name", 0);
            if (ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(mProduct.Name);
                mProduct.Slug = strSlug;

                mProduct.Updated_at = DateTime.Now;
                mProduct.Updated_by = 1;

                // Upload file
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    mProduct.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Public/images/products/"), filename);
                    file.SaveAs(Strpath);
                }
               
                db.Entry(mProduct).State = EntityState.Modified;
                db.SaveChanges();
                Notification.set_flash("Đã cập nhật lại thông tin sản phẩm!", "success");
                return RedirectToAction("Index");
            }
            return View(mProduct);
        }
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        public ActionResult DelTrash(int? id)
        {
            mProduct mProduct = db.Product.Find(id);
            mProduct.Status = 0;

            mProduct.Updated_at = DateTime.Now;
            mProduct.Updated_by = 1;
            db.Entry(mProduct).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Ném thành công vào thùng rác!" + " ID = " + id, "success");
            return RedirectToAction("Index");
        }
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        public ActionResult Undo(int? id)
        {
            mProduct mProduct = db.Product.Find(id);
            mProduct.Status = 2;

            mProduct.Updated_at = DateTime.Now;
            mProduct.Updated_by = 1;
            db.Entry(mProduct).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Khôi phục thành công!" + " ID = " + id, "success");
            return RedirectToAction("Trash");
        }
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại !", "warning");
                return RedirectToAction("Trash");
            }
            mProduct mProduct = db.Product.Find(id);
            if (mProduct == null)
            {
                Notification.set_flash("Không tồn tại !", "warning");
                return RedirectToAction("Trash");
            }
            return View(mProduct);
        }
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            mProduct mProduct = db.Product.Find(id);
            db.Product.Remove(mProduct);
            db.SaveChanges();
            Notification.set_flash("Đã xóa vĩnh viễn sản phẩm!", "danger");
            return RedirectToAction("Trash");
        }
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        [HttpPost]
        public JsonResult changeStatus(int id)
        {
            mProduct mProduct = db.Product.Find(id);
            mProduct.Status = (mProduct.Status == 1) ? 2 : 1;

            mProduct.Updated_at = DateTime.Now;
            mProduct.Updated_by = 1;
            db.Entry(mProduct).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Status = mProduct.Status });
        }
    }
}
