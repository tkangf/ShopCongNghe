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
    public class MenuController : BaseController
    {

        private TranAnhShopDbContext db = new TranAnhShopDbContext();

        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Index()
        {
            ViewBag.countTrash = db.Menu.Where(m => m.Status == 0).Count();
            ViewBag.ListCat = db.Category.Where(m => m.Status == 1).ToList();
            ViewBag.ListTopic = db.Topic.Where(m => m.Status == 1).ToList();
            ViewBag.ListPage = db.Post.Where(m => m.Status == 1 && m.Type == "page").ToList();
            return View(db.Menu.Where(m => m.Status != 0).ToList());
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        [HttpPost]
        public ActionResult Index(FormCollection data)
        {
            ViewBag.ListCat = db.Category.Where(m => m.Status == 1).ToList();
            ViewBag.ListTopic = db.Topic.Where(m => m.Status == 1).ToList();
            ViewBag.ListPage = db.Post.Where(m => m.Status == 1 && m.Type == "page").ToList();

            if (!String.IsNullOrEmpty(data["AddCat"]))
            {
                if (!String.IsNullOrEmpty(data["itemCat"]))
                {
                    var itemcat = data["itemCat"];
                    var arr = itemcat.Split(',');
                    int count = 0;
                    foreach (var i in arr)
                    {
                        int id = int.Parse(i);
                        mCategory mCategory = db.Category.Find(id);
                        mMenu mMenu = new mMenu();
                        mMenu.Name = mCategory.Name;
                        mMenu.Link = mCategory.Slug;
                        mMenu.Type = "category";
                        mMenu.TableID = id;
                        mMenu.Orders = 1;
                        mMenu.Positon = data["Position"];
                        mMenu.ParentID = 0;
                        mMenu.Status = 2;
                        mMenu.Created_at = DateTime.Now;
                        mMenu.Updated_by = int.Parse(Session["Admin_ID"].ToString());
                        mMenu.Updated_at = DateTime.Now;
                        mMenu.Updated_by = int.Parse(Session["Admin_ID"].ToString());
                        db.Menu.Add(mMenu);
                        db.SaveChanges();
                        count++;
                    }
                    Notification.set_flash("Đã thêm " + count + " menu mới!", "success");
                }
                else
                {
                    Notification.set_flash("Chưa chọn menu cần thêm!", "warning");
                }
            }
            if (!String.IsNullOrEmpty(data["AddTopic"]))
            {
                if (!String.IsNullOrEmpty(data["itemTopic"]))
                {
                    var itemtopic = data["itemTopic"];
                    var arr = itemtopic.Split(',');
                    int count = 0;
                    foreach (var i in arr)
                    {
                        int id = int.Parse(i);
                        mTopic mTopic = db.Topic.Find(id);
                        mMenu mMenu = new mMenu();
                        mMenu.Name = mTopic.Name;
                        mMenu.Link = mTopic.Slug;
                        mMenu.Type = "topic";
                        mMenu.TableID = id;
                        mMenu.Orders = 1;
                        mMenu.Positon = data["Position"];
                        mMenu.ParentID = 0;
                        mMenu.Status = 2;
                        mMenu.Created_at = DateTime.Now;
                        mMenu.Updated_by = int.Parse(Session["Admin_ID"].ToString());
                        mMenu.Updated_at = DateTime.Now;
                        mMenu.Updated_by = int.Parse(Session["Admin_ID"].ToString());
                        db.Menu.Add(mMenu);
                        db.SaveChanges();
                        count++;
                    }
                    Notification.set_flash("Đã thêm " + count + " menu mới!", "success");
                }
                else
                {
                    Notification.set_flash("Chưa chọn menu cần thêm!", "warning");
                }
            }
            if (!String.IsNullOrEmpty(data["AddPage"]))
            {

                if (!String.IsNullOrEmpty(data["itemPage"]))
                {
                    var itempage = data["itemPage"];
                    var arr = itempage.Split(',');
                    int count = 0;
                    foreach (var i in arr)
                    {
                        int id = int.Parse(i);
                        mPost mPost = db.Post.Find(id);
                        mMenu mMenu = new mMenu();
                        mMenu.Name = mPost.Title;
                        mMenu.Link = mPost.Slug;
                        mMenu.Type = "page";
                        mMenu.TableID = id;
                        mMenu.Orders = 1;
                        mMenu.Positon = data["Position"];
                        mMenu.ParentID = 0;
                        mMenu.Status = 2;
                        mMenu.Created_at = DateTime.Now;
                        mMenu.Updated_by = int.Parse(Session["Admin_ID"].ToString());
                        mMenu.Updated_at = DateTime.Now;
                        mMenu.Updated_by = int.Parse(Session["Admin_ID"].ToString());
                        db.Menu.Add(mMenu);
                        db.SaveChanges();
                        count++;
                    }
                    Notification.set_flash("Đã thêm " + count + " menu mới!", "success");
                }
                else
                {
                    Notification.set_flash("Chưa chọn menu cần thêm!", "warning");
                }
            }
            if (!String.IsNullOrEmpty(data["AddCustom"]))
            {
                if (!String.IsNullOrEmpty(data["name"]) && !String.IsNullOrEmpty(data["link"]))
                {
                    mMenu mMenu = new mMenu();
                    mMenu.Name = data["name"];
                    mMenu.Link = data["link"];
                    mMenu.Type = "custom";
                    mMenu.Orders = 1;
                    mMenu.Positon = data["Position"];
                    mMenu.ParentID = 0;
                    mMenu.Status = 2;
                    mMenu.Created_at = DateTime.Now;
                    mMenu.Updated_by = int.Parse(Session["Admin_ID"].ToString());
                    mMenu.Updated_at = DateTime.Now;
                    mMenu.Updated_by = int.Parse(Session["Admin_ID"].ToString());
                    db.Menu.Add(mMenu);
                    db.SaveChanges();
                    Notification.set_flash("Đã thêm 1 menu mới!", "success");
                }
                else
                {
                    Notification.set_flash("Vui lòng nhập tên menu và link!", "warning");
                }
            }

            return View(db.Menu.Where(m => m.Status != 0).ToList());
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Trash()
        {
            return View(db.Menu.Where(m => m.Status == 0).ToList());
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            mMenu mMenu = db.Menu.Find(id);
            if (mMenu == null)
            {
                Notification.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            return View(mMenu);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult DelTrash(int id)
        {
            mMenu mMenu = db.Menu.Find(id);
            if (mMenu == null)
            {
                Notification.set_flash("Không tồn tại danh mục cần xóa vĩnh viễn!", "warning");
                return RedirectToAction("Index");
            }

            mMenu.Status = 0;
            mMenu.Updated_at = DateTime.Now;
            mMenu.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(mMenu).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Ném thành công vào thùng rác!" + " ID = " + id, "success");
            return RedirectToAction("Index");
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult ReTrash(int? id)
        {
            mMenu mMenu = db.Menu.Find(id);
            if (mMenu == null)
            {
                Notification.set_flash("Không tồn tại danh mục!", "warning");
                return RedirectToAction("Trash", "Category");
            }
            mMenu.Status = 2;

            mMenu.Updated_at = DateTime.Now;
            mMenu.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(mMenu).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Khôi phục thành công!" + " ID = " + id, "success");
            return RedirectToAction("Trash", "Menu");
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            mMenu mMenu = db.Menu.Find(id);
            if (mMenu == null)
            {
                Notification.set_flash("Không tồn tại!", "warning");
                return RedirectToAction("Index");
            }
            ViewBag.parentMenu = new SelectList(db.Menu.Where(m => m.Status == 1 && m.ParentID == 0), "ID", "Name", 0);
            return View(mMenu);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(mMenu mMenu)
        {
            ViewBag.parentMenu = new SelectList(db.Menu.Where(m => m.Status == 1 && m.ParentID == 0), "ID", "Name", 0);
            if (ModelState.IsValid)
            {
                if (mMenu.ParentID == null)
                {
                    mMenu.ParentID = 0;
                }
                mMenu.Type = "page";
                mMenu.Orders = 1;
                mMenu.Updated_at = DateTime.Now;
                mMenu.Updated_by = int.Parse(Session["Admin_ID"].ToString());
                db.Entry(mMenu).State = EntityState.Modified;
                db.SaveChanges();
                Notification.set_flash("Cập nhật thành công!", "success");
                return RedirectToAction("Index");
            }
            return View(mMenu);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại menu!", "warning");
                return RedirectToAction("Trash", "Menu");
            }
            mMenu mMenu = db.Menu.Find(id);
            if (mMenu == null)
            {
                Notification.set_flash("Không tồn tại menu!", "warning");
                return RedirectToAction("Trash", "Menu");
            }

          
            return View(mMenu);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        // POST: Admin/Menu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            mMenu mMenu = db.Menu.Find(id);
            db.Menu.Remove(mMenu);
            db.SaveChanges();
            Notification.set_flash("Đã xóa vĩnh viễn menu!", "success");
            return RedirectToAction("Trash");
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        [HttpPost]
        public JsonResult changeStatus(int id)
        {
            mMenu mMenu = db.Menu.Find(id);
            mMenu.Status = (mMenu.Status == 1) ? 2 : 1;

            mMenu.Updated_at = DateTime.Now;
            mMenu.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(mMenu).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new
            {
                Status = mMenu.Status
            });
        }

    }
}
