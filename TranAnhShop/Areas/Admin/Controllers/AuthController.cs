using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TranAnhShop.Models;
using System.Web.Mvc;
using TranAnhShop.Common;

namespace TranAnhShop.Areas.Admin.Controllers
{
    public class AuthController : Controller
    {
        private TranAnhShopDbContext db = new TranAnhShopDbContext();
        public ActionResult Login()
        {
            if (Session["Admin_Name"] != null)
            {
                Response.Redirect("~/Admin");
            }
            return View();
        }
        [HttpPost]
        public JsonResult Login(String User, String Pass)
        {
            int count_username = db.User.Where(m => m.Status == 1 && ((m.Phone).ToString() == User|| m.Email == User || m.Name == User) && m.Access != 1).Count();
            if (count_username == 0)
                return Json(new { s = 1 });
            else
            {
                String Password = XString.ToMD5(Pass);
                //String Password = Pass;
                var user_acount = db.User
                .Where(m => m.Status == 1 && ((m.Phone).ToString() == User || m.Email == User || m.Name == User) && m.Access != 1 && m.Password == Password);
                if (user_acount.Count() == 0)
                    return Json(new { s = 2 });
                else
                {
                    var user = user_acount.First();

                    Session["Admin_fullname"] = user.Fullname;
                    Session["Admin_ID"] = user.ID;
                    Session["Admin_Name"] = user.Name;
                    Session["Admin_Images"] = user.Image;
                    Session["Admin_Address"] = user.Address;
                    Session["Admin_Phone"] = user.Phone;
                    role role = db.Roles.Where(m => m.parentId == user.Access).First();
                    var userSession = new Userlogin();
                    userSession.UserName = user.Name;
                    userSession.UserID = user.ID;
                    userSession.GroupID = role.GropID;
                    userSession.AccessName = role.accessName;
                    Session.Add(CommonConstants.USER_SESSION, userSession);
                    var i = Session["SESSION_CREDENTIALS"];
                    Session["Admin_ID"] = user.ID;
                    Session["Admin_Name"] = user.Name;
                    Session["Admin_fullname"] = user.Fullname;

                    return Json(new { s = 0 });
                }
            }
        }

        public ActionResult Logout()
        {
            if (Session["Admin_Name"] != null)
            {
                Session["Admin_Name"] = null;
                Session["Admin_ID"] = null;
            }
            return Redirect("~/Admin/Login");
        }
    }
}