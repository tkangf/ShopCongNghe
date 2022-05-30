using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TranAnhShop.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            if (System.Web.HttpContext.Current.Session["Admin_Name"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/Admin/Login");
            }
        }
        /*protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (Session["Admin_ID"].Equals(""))
            {
                RouteValueDictionary route = new RouteValueDictionary(new { Controller = "Auth", Action = "Login" });
                filterContext.Result = new RedirectToRouteResult(route);
                return;
            }
        }*/
    }
}