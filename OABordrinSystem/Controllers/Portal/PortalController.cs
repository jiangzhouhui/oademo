using OABordrinCommon;
using OABordrinSystem.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OABordrinSystem.Controllers.Portal
{
    [PageAuthorizationFilter]
    public class PortalController :BaseController
    {
        // GET: Portal
        public ActionResult Index()
        {
            ViewBag.CurrentName = "主页";
            return View();
        }
    }
}