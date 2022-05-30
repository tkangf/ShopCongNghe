using TranAnhShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using System.IO;
using TranAnhShop;
using TranAnhShop.Common;
using System.Data.Entity;

namespace TranAnhShop.Controllers
{
    public class AccountController : Controller
    {
        private TranAnhShopDbContext db = new TranAnhShopDbContext();
        //public AccountController()
        //{
        //    if (System.Web.HttpContext.Current.Session["User_Name"] == null)
        //    {
        //        System.Web.HttpContext.Current.Response.Redirect("~/");
        //    }
        //}

        [HttpPost]
        public JsonResult UserLogin(String User, String Password)
        {
            int count_username = db.User.Where(m => m.Status == 1 &&
            ((m.Phone).ToString() == User || m.Email == User || m.Name == User)
            && m.Access == 1).Count();
            if (count_username == 0)
            {

                return Json(new { s = 1 });
            }
            else
            {
                Password = XString.ToMD5(Password);

                var user_acount = db.User.Where(m => m.Status == 1 && ((m.Phone).ToString() == User || m.Email == User || m.Name == User) && m.Password == Password);
                if (user_acount.Count() == 0)
                {
                    return Json(new { s = 2 });
                }
                else
                {
                    var user = user_acount.First();
                    Session["User_Name"] = user.Fullname;
                    Session["User_ID"] = user.ID;
                    Session["Email_Name"] = user.Email;
                    Session["Address_Name"] = user.Address;
                }
            }
            return Json(new { s = 0 });
        }

        public ActionResult UserLogout(String url)
        {
            Session["User_Name"] = null;
            Session["User_ID"] = null;
            return Redirect("~/" + url);
        }
        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (TranAnhShopDbContext dc = new TranAnhShopDbContext())
            {
                var v = dc.User.Where(a => a.EmailID == emailID).FirstOrDefault();
                return v != null;
            }
        }

        public ActionResult ProFile(String User, String Password)
        {


            var list = db.User.Where(m => m.Status == 1 && m.Access == 1).Take(1).OrderByDescending(m => m.Created_at);

            var user_acount = db.User
                .Where(m => m.Status == 1 && ((m.Phone).ToString() == User || m.Email == User || m.Name == User)
                && m.Access == 1 && m.Password == Password);

            /*var user = user_acount.First();

            
            Session["User_Name"] = user.Fullname;*/

            var hienthi = db.User.Where(m => m.Status == 1).First();
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }

            return View(list);
        }
        /*[HttpPost]
        *//*[ValidateAntiForgeryToken]*//*
        public ActionResult ProFile([Bind(Exclude = "IsEmailVerified,ActivationCode")] mUser user)
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            *//* bool Status = false;
             string message = "";*/
        /*String avatar = XString.ToAscii(user.Fullname);*//*
        try
        {
            *//*var isExist = IsEmailExist(user.EmailID);
            if (isExist)
            {
                return Json(new { Code = 1, Message = "Email tồn tại!" });
            }
*//*
                var checkPM = db.User.Any(m => m.Phone == user.Phone && m.Email.ToLower().Equals(user.Email.ToLower()));
                if (checkPM)
                {
                    return Json(new { Code = 1, Message = "Số điện thoại hoặc Email đã được sử dụng." });
                }

                #region Generate Activation Code 
                user.ActivationCode = Guid.NewGuid();
                #endregion
                user.IsEmailVerified = false;
                #region Save to Database

                user.Gender = 0;
                user.Image = "~/Public/images/user/dummy-user-img.png";
                user.Access = 1;
                user.Status = 1;
                user.Password = XString.ToMD5(user.Password);
                user.Created_at = DateTime.Now;
                user.Created_by = 1;
                user.Updated_at = DateTime.Now;
                user.Updated_by = 1;
                *//*var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = avatar + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    user.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Public/images/user/"), filename);
                    file.SaveAs(Strpath);
                }*/
        /*using (TranAnhShopDbContext dc = new TranAnhShopDbContext())
        {
            *//* mUser.Created_at = DateTime.Now;
             mUser.Created_by = 1;
             mUser.Updated_at = DateTime.Now;
             mUser.Updated_by = 1;
             dc.User.Add(mUser);
             dc.SaveChanges();*//*
            //Send Email to User
            SendVerificationLinkEmail(user.EmailID, user.ActivationCode.ToString());
            *//*dc.Status = true;*/
        /*return Json(new
        {
            Code = 0,
            Message = "Đăng ký thành công. Liên kết kích hoạt tài khoản!" +
        " đã được gửi đến email của bạn:"
        });*//*
    }*//*
        #endregion
        *//*db.User.Add(user);*//*
        db.SaveChanges();


        return Json(new { Code = 0, Message = "Cập nhật thành công!" });
    }
    catch (Exception e)
    {
        return Json(new { Code = 1, Message = "Cập nhật thất bại!" });
        throw e;
    }
}*/
        public ActionResult Notification()
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            return View();
        }
        public ActionResult Order()
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            int userid = Convert.ToInt32(Session["User_ID"]);
            var list = db.Order.Where(m => m.UserID == userid).OrderByDescending(m => m.CreateDate).ToList();
            ViewBag.itemOrder = db.OrderDetail.ToList();
            ViewBag.productOrder = db.Product.ToList();
            return View(list);
        }
        public ActionResult ActionOrder()
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            var list = db.Order.ToList();
            ViewBag.Hoanthanh = db.Order.Where(m => m.Status == 3).Count();
            ViewBag.ChoXuLy = db.Order.Where(m => m.Status == 1).Count();
            ViewBag.DangXuLy = db.Order.Where(m => m.Status == 2).Count();
            return View("_ActionOrder", list);
        }
        public ActionResult OrderDetails(int id)
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            int userid = Convert.ToInt32(Session["User_ID"]);
            var checkO = db.Order.Where(m => m.UserID == userid && m.ID == id);
            if (checkO.Count() == 0)
            {
                return this.NotFound();
            }

            var id_order = db.Order.Where(m => m.UserID == userid && m.ID == id).FirstOrDefault();
            ViewBag.Order = id_order;
            var itemOrder = db.OrderDetail.Where(m => m.OrderID == id_order.ID).ToList();
            ViewBag.productOrder = db.Product.ToList();
            return View(itemOrder);
        }
        public ActionResult NotFound()
        {
            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            return View();
        }
        [HttpPost]
        public JsonResult Register([Bind(Exclude = "IsEmailVerified,ActivationCode")] mUser user)
        {

            /* bool Status = false;
             string message = "";*/
            /*String avatar = XString.ToAscii(user.Fullname);*/
            try
            {
                /*var isExist = IsEmailExist(user.EmailID);
                if (isExist)
                {
                    return Json(new { Code = 1, Message = "Email tồn tại!" });
                }
*/
                var checkPM = db.User.Any(m => m.Phone == user.Phone && m.Email.ToLower().Equals(user.Email.ToLower()));
                if (checkPM)
                {
                    return Json(new { Code = 1, Message = "Số điện thoại hoặc Email đã được sử dụng." });
                }

                #region Generate Activation Code 
                user.ActivationCode = Guid.NewGuid();
                #endregion
                user.IsEmailVerified = false;
                #region Save to Database

                user.Gender = 0;
                user.Image = "~/Public/images/user/dummy-user-img.png";
                user.Access = 1;
                user.Status = 1;
                user.Password = XString.ToMD5(user.Password);
                user.Created_at = DateTime.Now;
                user.Created_by = 1;
                user.Updated_at = DateTime.Now;
                user.Updated_by = 1;
                /*var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = avatar + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    user.Image = filename;
                    String Strpath = Path.Combine(Server.MapPath("~/Public/images/user/"), filename);
                    file.SaveAs(Strpath);
                }*/
                /*using (TranAnhShopDbContext dc = new TranAnhShopDbContext())
                {
                    *//* mUser.Created_at = DateTime.Now;
                     mUser.Created_by = 1;
                     mUser.Updated_at = DateTime.Now;
                     mUser.Updated_by = 1;
                     dc.User.Add(mUser);
                     dc.SaveChanges();*//*
                    //Send Email to User
                    SendVerificationLinkEmail(user.EmailID, user.ActivationCode.ToString());
                    *//*dc.Status = true;*/
                /*return Json(new
                {
                    Code = 0,
                    Message = "Đăng ký thành công. Liên kết kích hoạt tài khoản!" +
                " đã được gửi đến email của bạn:"
                });*//*
            }*/
                #endregion
                db.User.Add(user);
                db.SaveChanges();


                return Json(new { Code = 0, Message = "Đăng ký thành công!" });
            }
            catch (Exception e)
            {
                return Json(new { Code = 1, Message = "Đăng ký thất bại!" });
                throw e;
            }
        }
        /*[NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (TranAnhShopDbContext dc = new TranAnhShopDbContext())
            {
                var v = dc.User.Where(a => a.EmailID == emailID).FirstOrDefault();
                return v != null;
            }
        }*/
        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("testmailaspdotnet@gmail.com", "TranAnhShop");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "Mk123456"; // Replace with actual password

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Tài khoản của bạn đã được tạo thành công!";
                body = "<br/><br/>Chúng tôi rất vui khi được thông báo với bạn rằng tài khoản TranAnhShop của bạn là" +
                    " thành công trong việc tạo ra. Vui lòng nhấp vào liên kết dưới đây để xác minh tài khoản của bạn" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";

            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Đặt lại mật khẩu";
                body = "Xin chào,<br/><br/>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu tài khoản của bạn. Vui lòng nhấp vào liên kết dưới đây để đặt lại mật khẩu của bạn" +
                    "<br/><br/><a href=" + link + ">Đặt lại liên kết mật khẩu</a>";
            }


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                Timeout = 10000,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
        public ActionResult EditCustomer(int? id)
        {

            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
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
        public ActionResult EditCustomer(mUser customer, String pass)
        {

            if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            if (ModelState.IsValid)
            {
                customer.Gender = 0;
                customer.Image = "~/Public/images/user/dummy-user-img.png";
                customer.Access = 1;
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
                return RedirectToAction("DoneEdit");
            }

            return View("DoneEdit");
        }
        public ActionResult DoneEdit()
        {
            return View();
        }
        public ActionResult Info()
        {
            /*if (System.Web.HttpContext.Current.Session["User_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
            int userid = Convert.ToInt32(Session["User_ID"]);
            var list = db.Order.Where(m => m.UserID == userid).OrderByDescending(m => m.CreateDate).ToList();
            ViewBag.itemOrder = db.OrderDetail.ToList();
            int user_id = Convert.ToInt32(Session["User_ID"]);
            ViewBag.Info = db.User.Where(m => m.ID == user_id).First();
            ViewBag.productOrder = db.Product.ToList();*/
            var list = db.User.Where(m => m.Status==1).First();
            return View(list);
        }
    }
}