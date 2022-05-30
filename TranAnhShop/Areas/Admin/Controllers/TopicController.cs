using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TranAnhShop.Common;
using TranAnhShop.Models;

namespace TranAnhShop.Areas.Admin.Controllers
{
    public class TopicController : BaseController
    {
        private TranAnhShopDbContext db = new TranAnhShopDbContext();
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        // GET: Admin/Topic
        public ActionResult Index()
        {
            ViewBag.countTrash = db.Topic.Where(m => m.Status == 0).Count();
            var list = db.Topic.Where(m => m.Status != 0).ToList();

            foreach (var row in list)
            {
                var temp_link = db.Link.Where(m => m.Type == "topic" && m.TableID == row.ID);
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
                    row_link.Type = "topic";
                    row_link.TableID = row.ID;
                    db.Link.Add(row_link);
                }
            }
            db.SaveChanges();
            return View(list);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]

        // GET: Admin/Topic/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            mTopic mTopic = db.Topic.Find(id);
            if (mTopic == null)
            {
                return HttpNotFound();
            }
            return View(mTopic);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Status(int? id)
        {
            mTopic mTopic = db.Topic.Find(id);
            if (mTopic == null)
            {
                Notification.set_flash("Không tồn tại danh mục!", "warning");
                return RedirectToAction("Index");
            }
            mTopic.Status = (mTopic.Status == 1) ? 2 : 1;

            mTopic.Updated_at = DateTime.Now;
            mTopic.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(mTopic).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Thay đổi trạng thái thành công!" + " id = " + id, "success");
            return RedirectToAction("Index");
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Create()
        {
            ViewBag.listTopic = new SelectList(db.Topic.Where(m => m.Status == 1), "ID", "Name", 0);
            ViewBag.listOrder = new SelectList(db.Topic.Where(m => m.Status == 1), "Order", "Name", 0);
            return View();
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(mTopic mTopic)
        {
            ViewBag.listTopic = new SelectList(db.Topic.Where(m => m.Status == 1), "ID", "Name", 0);
            ViewBag.listOrder = new SelectList(db.Topic.Where(m => m.Status == 1), "Order", "Name", 0);
            if (ModelState.IsValid)
            {
                if(mTopic.ParentID == null)
                {
                    mTopic.ParentID = 0;
                }
                String Slug = XString.ToAscii(mTopic.Name);
                if (db.Category.Where(m => m.Slug == Slug).Count() > 0)
                {
                    Notification.set_flash("Tên danh mục đã tồn tại, vui lòng thử lại!", "warning");
                    return RedirectToAction("Create", "Topic");
                }
                if (db.Topic.Where(m => m.Slug == Slug).Count() > 0)
                {
                    Notification.set_flash("Tên danh mục đã tồn tại trong TOPIC, vui lòng thử lại!", "warning");
                    return RedirectToAction("Create", "Topic");
                }
                if (db.Post.Where(m => m.Slug == Slug).Count() > 0)
                {
                    Notification.set_flash("Tên danh mục đã tồn tại trong POST, vui lòng thử lại!", "warning");
                    return RedirectToAction("Create", "Topic");
                }
                if (db.Product.Where(m => m.Slug == Slug).Count() > 0)
                {
                    Notification.set_flash("Tên danh mục đã tồn tại trong PRODUCT, vui lòng thử lại!", "warning");
                    return RedirectToAction("Create", "Topic");
                }


                mTopic.Slug = Slug;
                mTopic.Created_at = DateTime.Now;
                mTopic.Created_by = int.Parse(Session["Admin_ID"].ToString());
                mTopic.Updated_at = DateTime.Now;
                mTopic.Updated_by = int.Parse(Session["Admin_ID"].ToString());

                db.Topic.Add(mTopic);
                db.SaveChanges();
                Notification.set_flash("Danh mục đã được thêm!", "success");
                return RedirectToAction("Index", "Topic");
            }
            ViewBag.list = db.Category.Where(m => m.Status == 1).ToList();
            Notification.set_flash("Có lỗi xảy ra khi thêm danh mục!", "warning");
            return View(mTopic);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult DelTrash(int id)
        {
            mTopic mTopic = db.Topic.Find(id);
            if (mTopic == null)
            {
                Notification.set_flash("Không tồn tại danh mục cần xóa vĩnh viễn!", "warning");
                return RedirectToAction("Index");
            }
            int count_child = db.Topic.Where(m => m.ParentID == id).Count();
            if (count_child != 0)
            {
                Notification.set_flash("Không thể xóa, danh mục có chủ đề con!", "warning");
                return RedirectToAction("Index");
            }
            mTopic.Status = 0;

            mTopic.Updated_at = DateTime.Now;
            mTopic.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(mTopic).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Ném thành công vào thùng rác!" + " ID = " + id, "success");
            return RedirectToAction("Index");
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult ReTrash(int? id)
        {
            mTopic cate = db.Topic.Find(id);
            if (cate == null)
            {
                Notification.set_flash("Không tồn tại chủ đề!", "danger");
                return RedirectToAction("Trash", "Topic");
            }
            cate.Status = 2;

            cate.Updated_at = DateTime.Now;
            cate.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(cate).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Khôi phục thành công!" + " ID = " + id, "success");
            return RedirectToAction("Trash", "Topic");
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại !", "warning");
                return RedirectToAction("Trash", "Topic");
            }
            mTopic mTopic = db.Topic.Find(id);
            if (mTopic == null)
            {
                Notification.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Trash", "Topic");
            }
            return View(mTopic);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            mTopic mTopic = db.Topic.Find(id);
            db.Topic.Remove(mTopic);
            db.SaveChanges();
            Notification.set_flash("Đã xóa hoàn toàn chủ đề!", "success");
            return RedirectToAction("Trash", "Topic");
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Trash()
        {
            return View(db.Topic.Where(m=>m.Status == 0).ToList());
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Edit(int? id)
        {
            ViewBag.listTopic = new SelectList(db.Topic.Where(m => m.Status == 1), "ID", "Name", 0);
            ViewBag.listOrder = new SelectList(db.Topic.Where(m => m.Status == 1), "Order", "Name", 0);
            mTopic mTopic = db.Topic.Find(id);
            if (mTopic == null)
            {
                Notification.set_flash("404!", "warning");
                return RedirectToAction("Index", "Topic");
            }
            return View(mTopic);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(mTopic mTopic)
        {
            ViewBag.listTopic = new SelectList(db.Topic.Where(m => m.Status == 1), "ID", "Name", 0);
            ViewBag.listOrder = new SelectList(db.Topic.Where(m => m.Status == 1), "Order", "Name", 0);
            if (ModelState.IsValid)
            {
                if (mTopic.ParentID == null)
                {
                    mTopic.ParentID = 0;
                }
                String Slug = XString.ToAscii(mTopic.Name);
                int ID = mTopic.ID;
                if (db.Category.Where(m => m.Slug == Slug && m.ID != ID).Count() > 0)
                {
                    Notification.set_flash("Tên danh mục đã tồn tại, vui lòng thử lại!", "warning");
                    return RedirectToAction("Edit", "Topic");
                }
                if (db.Topic.Where(m => m.Slug == Slug && m.ID != ID).Count() > 0)
                {
                    Notification.set_flash("Tên danh mục đã tồn tại trong TOPIC, vui lòng thử lại!", "warning");
                    return RedirectToAction("Edit", "Topic");
                }
                if (db.Post.Where(m => m.Slug == Slug && m.ID != ID).Count() > 0)
                {
                    Notification.set_flash("Tên danh mục đã tồn tại trong POST, vui lòng thử lại!", "warning");
                    return RedirectToAction("Edit", "Topic");
                }
                if (db.Product.Where(m => m.Slug == Slug && m.ID != ID).Count() > 0)
                {
                    Notification.set_flash("Tên danh mục đã tồn tại trong PRODUCT, vui lòng thử lại!", "warning");
                    return RedirectToAction("Edit", "Topic");
                }

                mTopic.Slug = Slug;

                // Lỗi datatime2
                mTopic.Created_at = DateTime.Now;
                mTopic.Created_by = int.Parse(Session["Admin_ID"].ToString());

                mTopic.Updated_at = DateTime.Now;
                mTopic.Updated_by = int.Parse(Session["Admin_ID"].ToString());

                db.Entry(mTopic).State = EntityState.Modified;
                db.SaveChanges();
                Notification.set_flash("Câp nhật thành công chủ đề!", "success");
                return RedirectToAction("Index");
            }
            ViewBag.list = db.Category.Where(m => m.Status == 1).ToList();
            return View(mTopic);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]

        [HttpPost]
        public JsonResult changeStatus(int id)
        {
            mTopic mTopic = db.Topic.Find(id);
            mTopic.Status = (mTopic.Status == 1) ? 2 : 1;

            mTopic.Updated_at = DateTime.Now;
            mTopic.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(mTopic).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Status = mTopic.Status });
        }
    }
}
