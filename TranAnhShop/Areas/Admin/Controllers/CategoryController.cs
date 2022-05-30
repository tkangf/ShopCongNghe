using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using TranAnhShop.Common;
using TranAnhShop.Models;

namespace TranAnhShop.Areas.Admin.Controllers
{

    public class CategoryController : BaseController
    {
        private TranAnhShopDbContext db = new TranAnhShopDbContext();

        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        // Index
        public ActionResult Index()
        {
            ViewBag.count_trash = db.Category.Where(m => m.Status == 0).Count();
            var list = db.Category.Where(m => m.Status != 0).ToList();
            ViewBag.GetAllCategory = list;
            foreach (var row in list)
            {
                var temp_link = db.Link.Where(m => m.Type == "category" && m.TableID == row.ID);
                if (temp_link.Count() > 0)
                {
                    var row_link = temp_link.First();
                    row_link.Name = row.Name;
                    row_link.Slug = row.Slug;
                    db.Entry(row_link).State = EntityState.Modified;
                }
                else
                {
                    var row_link = new mLink();
                    row_link.Name = row.Name;
                    row_link.Slug = row.Slug;
                    row_link.Type = "category";
                    row_link.TableID = row.ID;
                    db.Link.Add(row_link);
                }
            }
            db.SaveChanges();
            return View(list);
        }
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        // Trash
        public ActionResult Trash()
        {
            var list = db.Category.Where(m => m.Status == 0).ToList();
            return View(list);
        }
        public ActionResult DelTrash(int id)
        {
            mCategory mCategory = db.Category.Find(id);
            if (mCategory == null)
            {
                Notification.set_flash("Không tồn tại danh mục cần xóa vĩnh viễn!", "warning");
                return RedirectToAction("Index");
            }
            int count_child = db.Category.Where(m => m.ParentID == id).Count();
            if (count_child != 0)
            {
                Notification.set_flash("Không thể xóa, danh mục có chứa danh mục con!", "warning");
                return RedirectToAction("Index");
            }
            mCategory.Status = 0;

            mCategory.Created_at = DateTime.Now;
            mCategory.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            mCategory.Updated_at = DateTime.Now;
            mCategory.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(mCategory).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Ném thành công vào thùng rác!" + " ID = " + id, "success");
            return RedirectToAction("Index");
        }
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        public ActionResult ReTrash(int? id)
        {
            mCategory mCategory = db.Category.Find(id);
            if (mCategory == null)
            {
                Notification.set_flash("Không tồn tại danh mục!", "warning");
                return RedirectToAction("Trash", "Category");
            }
            mCategory.Status = 2;

            mCategory.Updated_at = DateTime.Now;
            mCategory.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(mCategory).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Khôi phục thành công!" + " ID = " + id, "success");
            return RedirectToAction("Trash", "Category");
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại danh mục cần xóa!", "warning");
                return RedirectToAction("Trash", "Category");
            }
            mCategory mCategory = db.Category.Find(id);
            if (mCategory == null)
            {
                Notification.set_flash("Không tồn tại danh mục cần xóa!", "warning");
                return RedirectToAction("Trash", "Category");
            }
            return View(mCategory);
        }

        [HttpPost, ActionName("Delete")]
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            mCategory mCategory = db.Category.Find(id);
            db.Category.Remove(mCategory);
            db.SaveChanges();
            Notification.set_flash("Đã xóa hoàn toàn danh mục!", "success");
            return RedirectToAction("Trash", "Category");
        }
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            mCategory mCategory = db.Category.Find(id);
            if (mCategory == null)
            {
                Notification.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            return View(mCategory);
        }
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        public ActionResult Create()
        {
            ViewBag.listCat = new SelectList(db.Category.Where(m => m.Status == 1), "ID", "Name", 0);
            ViewBag.listOrder = new SelectList(db.Category.Where(m => m.Status == 1), "Orders", "Name", 0);

            return View();
        }
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(mCategory mCategory)
        {
            ViewBag.listCat = new SelectList(db.Category.Where(m => m.Status == 1), "ID", "Name", 0);
            ViewBag.listOrder = new SelectList(db.Category.Where(m => m.Status == 1), "Orders", "Name", 0);
            if (ModelState.IsValid)
            {
                if(mCategory.ParentID == 0)
                {
                    mCategory.ParentID = 0;
                }
                String Slug = XString.ToAscii(mCategory.Name);
                CheckSlug check = new CheckSlug();

                if (!check.KiemTraSlug("Category", Slug, null))
                {
                    Notification.set_flash("Tên danh mục đã tồn tại, vui lòng thử lại!", "warning");
                    return RedirectToAction("Create", "Category");
                }

                mCategory.Slug = Slug;
                mCategory.Created_at = DateTime.Now;
                mCategory.Created_by = int.Parse(Session["Admin_ID"].ToString());
                mCategory.Updated_at = DateTime.Now;
                mCategory.Updated_by = int.Parse(Session["Admin_ID"].ToString());

                db.Category.Add(mCategory);
                db.SaveChanges();
                Notification.set_flash("Danh mục đã được thêm!", "success");
                return RedirectToAction("Index", "Category");
            }

            Notification.set_flash("Có lỗi xảy ra khi thêm danh mục!", "warning");
            return View(mCategory);
        }

        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Edit(int? id)
        {
            ViewBag.listCat = new SelectList(db.Category.Where(m => m.Status == 1), "ID", "Name", 0);
            ViewBag.listOrder = new SelectList(db.Category.Where(m => m.Status == 1), "Orders", "Name", 0);
            mCategory mCategory = db.Category.Find(id);
            if (mCategory == null)
            {
                Notification.set_flash("404!", "warning");
                return RedirectToAction("Index", "Category");
            }
            return View(mCategory);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Edit(mCategory mCategory)
        {
            ViewBag.listCat = new SelectList(db.Category.Where(m => m.Status == 1), "ID", "Name", 0);
            ViewBag.listOrder = new SelectList(db.Category.Where(m => m.Status == 1), "Orders", "Name", 0);
            if (ModelState.IsValid)
            {
                String Slug = XString.ToAscii(mCategory.Name);
                int ID = mCategory.ID;
                if (db.Category.Where(m => m.Slug == Slug && m.ID != ID).Count() > 0)
                {
                    Notification.set_flash("Tên danh mục đã tồn tại, vui lòng thử lại!", "warning");
                    return RedirectToAction("Edit", "Category");
                }
                if (db.Topic.Where(m => m.Slug == Slug && m.ID != ID).Count() > 0)
                {
                    Notification.set_flash("Tên danh mục đã tồn tại trong TOPIC, vui lòng thử lại!", "warning");
                    return RedirectToAction("Edit", "Category");
                }
                if (db.Post.Where(m => m.Slug == Slug && m.ID != ID).Count() > 0)
                {
                    Notification.set_flash("Tên danh mục đã tồn tại trong POST, vui lòng thử lại!", "warning");
                    return RedirectToAction("Edit", "Category");
                }
                if (db.Product.Where(m => m.Slug == Slug && m.ID != ID).Count() > 0)
                {
                    Notification.set_flash("Tên danh mục đã tồn tại trong PRODUCT, vui lòng thử lại!", "warning");
                    return RedirectToAction("Edit", "Category");
                }

                mCategory.Slug = Slug;

                mCategory.Updated_at = DateTime.Now;
                mCategory.Updated_by = int.Parse(Session["Admin_ID"].ToString());

                db.Entry(mCategory).State = EntityState.Modified;
                db.SaveChanges();
                Notification.set_flash("Cập nhật thành công!", "success");
                return RedirectToAction("Index");
            }
            return View(mCategory);
        }

        [HttpPost]
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public JsonResult changeStatus(int id)
        {
            mCategory mCategory = db.Category.Find(id);
            mCategory.Status = (mCategory.Status == 1) ? 2 : 1;

            mCategory.Updated_at = DateTime.Now;
            mCategory.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(mCategory).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new
            {
                Status = mCategory.Status
            });
        }

    }
}
