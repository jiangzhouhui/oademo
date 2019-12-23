
using Aras.IOM;
using OABordrinBll;
using OABordrinCommon;
using OABordrinDAL;
using OABordrinEntity;
using OABordrinSystem.Utils;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Net.NetworkInformation;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace OABordrinSystem.Controllers.Help
{
    public class HelpController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult PRChinese()
        {
            return View();
        }
    }
}