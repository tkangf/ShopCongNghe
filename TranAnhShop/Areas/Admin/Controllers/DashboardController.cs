using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TranAnhShop.Models;

namespace TranAnhShop.Areas.Admin.Controllers
{
    public class DashboardController : BaseController
    {
        private TranAnhShopDbContext db = new TranAnhShopDbContext();
        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            DateTime hienTai = DateTime.Today;
            ViewBag.SoNguoiTruyCap = HttpContext.Application["SoNguoiTruyCap"].ToString();   //lấy slg ng truy cập từ application đã tạo
            ViewBag.SoNguoiDangOnline = HttpContext.Application["SoNguoiDangOnline"].ToString();   //lấy slg ng online từ application đã tạo
            ViewBag.TongDoanhThu = ThongKeDoanhThu();
            ViewBag.SoDonHang = ThongKeDonHang();
            ViewBag.SoThanhVien = ThongKeThanhVien();
            ViewBag.SoLuongSanPham = ThongKeSanPham();
            ViewBag.CountContactDoneReply = db.Contact.Where(m => m.Flag == 0).Count();
            ViewBag.CountOrderSuccess = db.Order.Where(m => m.Status == 3).Count();
            ViewBag.CountOrderCancel = db.Order.Where(m => m.Status == 1).Count();
     
            /* ViewBag.DoanhThuThang = ThongKeDoanhThuThang(hienTai.Month, hienTai.Year);*/
            return View();
        }
        public decimal ThongKeDoanhThu()
        {
            decimal TongDoanhThu = ((decimal)db.OrderDetail.Sum(m => m.Quantity * m.Price));
            return TongDoanhThu;
        }
        /* public decimal ThongKeDoanhThuThang(int Thang, int Nam)
         {
             var lstDDH = db.Order.Where(n => n.CreateDate.Month == Thang && n.CreateDate.Year == Nam);  //lấy ds đơn hàng có date tương ứng
             decimal TongDoanhThu = 0;
             foreach (var item in lstDDH) //duyệt chi tiết từng đơn và tính tổng tiền
             {
                 TongDoanhThu += decimal.Parse(item.ChiTietDonDatHangs.Sum(n => n.SoLuong * n.Dongia).Value.ToString());
             }
             return TongDoanhThu;
         }*/
        //Thống kê tổng sản phẩm
        
        public double ThongKeSanPham()
        {
            double slsp = db.Product.Count();    //đếm số đơn hàng
            return slsp;
        }
        //Thống kê tổng đơn hàng
        public double ThongKeDonHang()
        {
            double slddh = db.Order.Count();    //đếm số đơn hàng
            return slddh;
        }
        public double ThongKeThanhVien()
        {
            double sltv = db.User.Count();    //đếm số thành viên
            return sltv;
        }
        public ActionResult Profiles()
        {

            return View();
        }
        public ActionResult EditCustomers(int? id)
        {
            ViewBag.CountOrderCancel = db.Order.Where(m => m.Status == 1).Count();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            mUser cus = db.User.Find(id);
            if (cus == null)
            {
                return HttpNotFound();
            }

            return View(cus);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCustomers(mUser customer, String pass)
        {

            ViewBag.CountOrderCancel = db.Order.Where(m => m.Status == 1).Count();

            if (ModelState.IsValid)
            {
                customer.Gender = 0;
                customer.Image = "dummy-user-img.png";
                customer.Access = 0;
                customer.Status = 1;
                /*customer.Password = pass;*/
                customer.IsEmailVerified = false;
                customer.ActivationCode = Guid.NewGuid();
                customer.Password = XString.ToMD5(customer.Password);

                customer.Created_at = DateTime.Now;
                customer.Created_by = 1;
                customer.Updated_at = DateTime.Now;
                customer.Updated_by = 1;
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return View();
            }

            return View();
        }
        public ActionResult Test()
        {
            return View();
        }
    }
    
}