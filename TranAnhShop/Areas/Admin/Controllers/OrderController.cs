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
    public class OrderController : BaseController
    {
        private TranAnhShopDbContext db = new TranAnhShopDbContext();
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        public ActionResult Index()
        {
            ViewBag.countTrash = db.Order.Where(m => m.Trash == 1).Count();
            var results = (from od in db.OrderDetail
                           join o in db.Order on od.OrderID equals o.ID
                           where o.Trash != 1

                           group od by new { od.OrderID, o } into groupb
                           orderby groupb.Key.o.CreateDate descending
                           select new zListOrder
                           {
                               IDO = groupb.Key.OrderID,
                               SAmount = groupb.Sum(m => m.Amount),
                               CustomerName = groupb.Key.o.DeliveryName,
                               Status = groupb.Key.o.Status,
                               CreateDate = groupb.Key.o.CreateDate,
                               ExportDate = groupb.Key.o.ExportDate,


                           });

            return View(results.ToList());
        }
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        public ActionResult Trash()
        {
            ViewBag.countTrash = db.Order.Where(m => m.Status == 0).Count();
            var results = (from od in db.OrderDetail
                           join o in db.Order on od.OrderID equals o.ID
                           where o.Trash == 1

                           group od by new { od.OrderID, o } into groupb
                           orderby groupb.Key.o.CreateDate descending
                           select new zListOrder
                           {
                               IDO = groupb.Key.OrderID,
                               SAmount = groupb.Sum(m => m.Amount),
                               CustomerName = groupb.Key.o.DeliveryName,
                               Status = groupb.Key.o.Status,
                               CreateDate = groupb.Key.o.CreateDate,
                               ExportDate = groupb.Key.o.ExportDate,


                           });

            return View(results.ToList());
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult DelTrash(int? id)
        {
            mOrder mOrder = db.Order.Find(id);
            mOrder.Trash = 1;

            mOrder.Updated_at = DateTime.Now;
            mOrder.Updated_by = 1;
            db.Entry(mOrder).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Đã hủy đơn hàng!" + " ID = " + id, "success");
            return RedirectToAction("Index");
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Undo(int? id)
        {
            mOrder mOrder = db.Order.Find(id);
            mOrder.Trash = 0;

            mOrder.Updated_at = DateTime.Now;
            mOrder.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(mOrder).State = EntityState.Modified;
            db.SaveChanges();
            Notification.set_flash("Khôi phục thành công!" + " ID = " + id, "success");
            return RedirectToAction("Trash");
        }
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại đơn hàng!", "warning");
                return RedirectToAction("Index", "Order");
            }
            mOrder mOrder = db.Order.Find(id);
            if (mOrder == null)
            {
                Notification.set_flash("Không tồn tại  đơn hàng!", "warning");
                return RedirectToAction("Index", "Order");
            }
            ViewBag.orderDetails = db.OrderDetail.Where(m => m.OrderID == id).ToList();
            ViewBag.productOrder = db.Product.ToList();
            return View(mOrder);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                Notification.set_flash("Không tồn tại đơn hàng!", "warning");
                return RedirectToAction("Trash", "Order");
            }
            mOrder mOrder = db.Order.Find(id);
            if (mOrder == null)
            {
                Notification.set_flash("Không tồn tại đơn hàng!", "warning");
                return RedirectToAction("Trash", "Order");
            }
            ViewBag.orderDetails = db.OrderDetail.Where(m => m.OrderID == id).ToList();
            ViewBag.productOrder = db.Product.ToList();
            return View(mOrder);
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            mOrder mOrder = db.Order.Find(id);
            db.Order.Remove(mOrder);
            db.SaveChanges();
            Notification.set_flash("Đã xóa đơn hàng!", "success");
            return RedirectToAction("Trash");
        }
        [CustomAuthorizeAttribute(RoleID = "ACCOUNTANT")]
        [HttpPost]
        public JsonResult changeStatus(int id, int op)
        {
            mOrder mOrder = db.Order.Find(id);
            if (op == 1) { mOrder.Status = 1; } else if (op == 2) { mOrder.Status = 2; } else { mOrder.Status = 3; }

            mOrder.ExportDate = DateTime.Now;
            mOrder.Updated_at = DateTime.Now;
            mOrder.Updated_by = int.Parse(Session["Admin_ID"].ToString());
            db.Entry(mOrder).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { s = mOrder.Status, t = mOrder.ExportDate.ToString() });
        }
        public ActionResult Infos()
        {
            var list = db.User.Where(m => m.Status == 1).First();
                return View(list);
        }
    }
}
