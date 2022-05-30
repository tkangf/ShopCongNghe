using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TranAnhShop.Common;
using TranAnhShop.Models;

namespace TranAnhShop.Areas.Admin.Controllers
{
    public class PageController : BaseController
    {
        private TranAnhShopDbContext db = new TranAnhShopDbContext();
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Index()
        {
            ViewBag.countTrash = db.Post.Where(m => m.Status == 0 && m.Type == "page").Count();
            var list = db.Post.Where(m => m.Status != 0 && m.Type == "page").ToList();
            foreach (var row in list)
            {
                var temp_link = db.Link.Where(m => m.Type == "page" && m.TableID == row.ID);
                if (temp_link.Count() > 0)
                {
                    var row_link = temp_link.First();
                    row_link.Name = row.Title;
                    row_link.Slug = row.Slug;
                    db.Entry(row_link).State = EntityState.Modified;
                }
                else
                {
                    var row_link = new mLink();
                    row_link.Name = row.Title;
                    row_link.Slug = row.Slug;
                    row_link.Type = "page";
                    row_link.TableID = row.ID;
                    db.Link.Add(row_link);
                }
            }
            db.SaveChanges();
            return View(list);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Trash()
        {
            return View(db.Post.Where(m => m.Status == 0 && m.Type == "page").ToList());
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại trang đơn!", "warning");
                return RedirectToAction("Index", "Page");
            }
            mPost mPost = db.Post.Find(id);
            if (mPost == null)
            {
                Notification.set_flash("Không tồn tại trang đơn!", "warning");
                return RedirectToAction("Index", "Page");
            }
            return View(mPost);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Create()
        {
            return View();
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(mPost mPost)
        {
            if (ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(mPost.Title);
                mPost.Slug = strSlug;
                mPost.Type = "page";
                mPost.Created_at = DateTime.Now;
                mPost.Created_by = int.Parse(Session["Admin_ID"].ToString());
                mPost.Updated_at = DateTime.Now;
                mPost.Updated_by = int.Parse(Session["Admin_ID"].ToString());
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    mPost.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Public/images/pages/"), filename);
                    file.SaveAs(Strpath);
                }
                db.Post.Add(mPost);
                db.SaveChanges();
                Notification.set_flash("Đã thêm trang đơn mới!", "success");
                return RedirectToAction("Index");
            }

            return View(mPost);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại trang đơn!", "warning");
                return RedirectToAction("Index", "Page");
            }
            mPost mPost = db.Post.Find(id);
            if (mPost == null)
            {
                Notification.set_flash("Không tồn tại trang đơn!", "warning");
                return RedirectToAction("Index", "Page");
            }
            return View(mPost);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(mPost mPost)
        {
            if (ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(mPost.Title);
                mPost.Slug = strSlug;
                mPost.Type = "page";
                mPost.Updated_at = DateTime.Now;
                mPost.Updated_by = int.Parse(Session["Admin_ID"].ToString());
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    mPost.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Public/images/pages/"), filename);
                    file.SaveAs(Strpath);
                }

                db.Entry(mPost).State = EntityState.Modified;
                db.SaveChanges();
                Notification.set_flash("Đã cập nhật lại nội dung trang đơn!", "success");
                return RedirectToAction("Index");
            }
            return View(mPost);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult DelTrash(int? id)
        {
            mPost mPost = db.Post.Find(id);
            mPost.Status = 0;

            mPost.Updated_at = DateTime.Now;
            mPost.Updated_by = 1;
            db.Entry(mPost).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Đã chuyển vào thùng rác!" + " ID = " + id, "success");
            return RedirectToAction("Index");
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Undo(int? id)
        {
            mPost mPost = db.Post.Find(id);
            mPost.Status = 2;

            mPost.Updated_at = DateTime.Now;
            mPost.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(mPost).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Khôi phục thành công!" + " ID = " + id, "success");
            return RedirectToAction("Trash");
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại trang đơn!", "warning");
                return RedirectToAction("Index", "Page");
            }
            mPost mPost = db.Post.Find(id);
            if (mPost == null)
            {
                Notification.set_flash("Không tồn tại trang đơn!", "warning");
                return RedirectToAction("Index", "Page");
            }
            return View(mPost);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            mPost mPost = db.Post.Find(id);
            db.Post.Remove(mPost);
            db.SaveChanges();
            Notification.set_flash("Đã xóa vĩnh viễn", "danger");
            return RedirectToAction("Trash");
        }

    }
}
