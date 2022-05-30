using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TranAnhShop.Areas.Admin.Controllers
{
    public class ModulesController : Controller
    {
        // GET: Admin/Modules
        public ActionResult Header()
        {
            return View("_Header");
        }
        public ActionResult Footer()
        {
            return View("_Footer");
        }
        public ActionResult Menu()
        {
            return View("_Menu");
        }
    }
}