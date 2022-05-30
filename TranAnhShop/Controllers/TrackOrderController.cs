using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TranAnhShop.Models;

namespace TranAnhShop.Controllers
{
    public class TrackOrderController : Controller
    {
        private TranAnhShopDbContext db = new TranAnhShopDbContext();
        // GET: TrackOrder
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {
            string phoneNumber = fc["phone"].ToString();
            var listOrder = db.Order.Where(m => m.DeliveryPhone.Equals(phoneNumber)).OrderByDescending(m => m.ID).ToList();
            return View("listOrders", listOrder);
        }
        public ActionResult DetailOrder(int id)
        {
            var listOrder = db.Order.Find(id);
            return View("DetailOrder", listOrder);
        }
        public ActionResult productDetailCheckOut(int orderId)
        {
            var list = db.OrderDetail.Where(m => m.OrderID == orderId).ToList();
            return View("_productDetailCheckOut", list);

        }
        public ActionResult subnameProduct(int id)
        {
            var list = db.Product.Find(id);
            return View("_subproductOrdersuccess", list);

        }
    }
}