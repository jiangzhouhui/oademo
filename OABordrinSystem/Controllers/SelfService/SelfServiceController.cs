using OABordrinSystem.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OABordrinSystem.Controllers.SelfService
{
    [PageAuthorizationFilter]
    public class SelfServiceController : Controller
    {
        // GET: SelfService
        public ActionResult Index()
        {
            ViewBag.CurrentName = "自助服务";
            return View();
        }
    }
}