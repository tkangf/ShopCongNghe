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
    public class PostController : BaseController
    {
        private TranAnhShopDbContext db = new TranAnhShopDbContext();
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Index()
        {
            ViewBag.countTrash = db.Post.Where(m => m.Status == 0 && m.Type == "post").Count();
            var list = from p in db.Post
                       join t in db.Topic
                       on p.TopicID equals t.ID
                       where p.Status != 0
                       orderby p.Created_at descending
                       select new PostTopic()
                       {
                           PostID = p.ID,
                           PostImage = p.Image,
                           PostName = p.Title,
                           PostStatus = p.Status,
                           TopicName = t.Name
                       };
            return View(list.ToList());
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Trash()
        {
            var list = from p in db.Post
                       join t in db.Topic
                       on p.TopicID equals t.ID
                       where p.Status == 0
                       orderby p.Created_at descending
                       select new PostTopic()
                       {
                           PostID = p.ID,
                           PostImage = p.Image,
                           PostName = p.Title,
                           PostStatus = p.Status,
                           TopicName = t.Name
                       };
            return View(list.ToList());
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        // Create
        public ActionResult Create()
        {
            mTopic mTopic = new mTopic();
            ViewBag.ListTopic = new SelectList(db.Topic.ToList(), "ID", "Name", 0);
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
                mPost.Type = "post";
                mPost.Created_at = DateTime.Now;
                mPost.Created_by = int.Parse(Session["Admin_ID"].ToString());
                mPost.Updated_at = DateTime.Now;
                mPost.Updated_by = int.Parse(Session["Admin_ID"].ToString());
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    mPost.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Public/images/post/"), filename);
                    file.SaveAs(Strpath);
                }
                db.Post.Add(mPost);
                db.SaveChanges();
                Notification.set_flash("Đã thêm bài viết mới!", "success");
                return RedirectToAction("Index");
            }
            return View(mPost);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        // Edit
        public ActionResult Edit(int? id)
        {
            mTopic mTopic = new mTopic();
            ViewBag.ListTopic = new SelectList(db.Topic.ToList(), "ID", "Name", 0);
            mPost mPost = db.Post.Find(id);
            if (mPost == null)
            {
                Notification.set_flash("Không tồn tại bài viết!", "warning");
                return RedirectToAction("Index", "Post");
            }
            return View(mPost);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(mPost mPost)
        {
            mTopic mTopic = new mTopic();
            ViewBag.ListTopic = new SelectList(db.Topic.ToList(), "ID", "Name", 0);
            if (ModelState.IsValid)
            {
                String strSlug = XString.ToAscii(mPost.Title);
                mPost.Slug = strSlug;
                mPost.Type = "post";
                mPost.Updated_at = DateTime.Now;
                mPost.Updated_by = int.Parse(Session["Admin_ID"].ToString());
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    mPost.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Public/images/post/"), filename);
                    file.SaveAs(Strpath);
                }

                db.Entry(mPost).State = EntityState.Modified;
                db.SaveChanges();
                Notification.set_flash("Đã cập nhật lại bài viết!", "success");
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
            mPost.Updated_by = int.Parse(Session["Admin_ID"].ToString());
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
        [HttpPost]
        public JsonResult changeStatus(int id)
        {
            mPost mPost = db.Post.Find(id);
            mPost.Status = (mPost.Status == 1) ? 2 : 1;

            mPost.Updated_at = DateTime.Now;
            mPost.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(mPost).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Status = mPost.Status });
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại bài viết!", "warning");
                return RedirectToAction("Index", "Post");
            }
            mPost mPost = db.Post.Find(id);
            if (mPost == null)
            {
                Notification.set_flash("Không tồn tại bài viết!", "warning");
                return RedirectToAction("Index", "Post");
            }
            return View(mPost);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại bài viết!", "warning");
                return RedirectToAction("Index", "Post");
            }
            mPost mPost = db.Post.Find(id);
            if (mPost == null)
            {
                Notification.set_flash("Không tồn tại bài viết!", "warning");
                return RedirectToAction("Index", "Post");
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
