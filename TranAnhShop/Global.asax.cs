using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace TranAnhShop
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            Application["SoNguoiTruyCap"] = 0;
            Application["SoNguoiDangOnline"] = 0;
        }
        protected void Session_Start()
        {
            Session["Notification"] = "";
            // Administrators
            Session["Admin_Name"] = null;
            Session["Admin_ID"] = null;
            Session["Admin_fullname"] = null;
            Session["Admin_Images"] = null;
            Session["Admin_Address"] = null;
            Session["Admin_Phone"] = null;

            // Customer
            Session["User_Name"] = null;
            Session["User_ID"] = null;
            Session["Cart"] = null;
            Session["keywords"] = null;
            Session["Address_Name"] = null;
            Session["Email_Name"] = null;
            Application.Lock();     //dùng đồng bộ hóa
            Application["SoNguoiTruyCap"] = (int)Application["SoNguoiTruyCap"] + 1;
            Application["SoNguoiDangOnline"] = (int)Application["SoNguoiDangOnline"] + 1;
            Application.UnLock();       ////dùng ngắt bộ hóa
        }
       
        protected void Session_End()
        {
            Application.Lock();
            Application["SoNguoiDangOnline"] = (int)Application["SoNguoiDangOnline"] - 1;
            Application.UnLock();
        }

        public void Application_End()
        {
            Application.Lock();
            Application["SoNguoiDangOnline"] = (int)Application["SoNguoiDangOnline"] - 1;
            Application.UnLock();
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            var TaiKhoanCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];  //truy vấn lấy cookie
            if (TaiKhoanCookie != null)  //ktra cookie dang nhap r
            {
                var authTicket = FormsAuthentication.Decrypt(TaiKhoanCookie.Value);     //decrypt cookie để lấy quyền
                var roles = authTicket.UserData.Split(new Char[] { ',' });      //lấy từng quyền
                var userPrincipal = new GenericPrincipal(new GenericIdentity(authTicket.Name), roles);  //gán role vào
                Context.User = userPrincipal;
            }
        }
    }
}
