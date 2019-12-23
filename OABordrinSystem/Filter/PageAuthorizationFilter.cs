using Aras.IOM;
using OABordrinEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OABordrinSystem.Filter
{
    public class PageAuthorizationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpCookie cookie = filterContext.RequestContext.HttpContext.Request.Cookies["Passport.Token"];
            
            if (cookie == null) // 站内凭证不存在
            {
                filterContext.Result = new RedirectResult("/Login/Index");
            }
          
            base.OnActionExecuting(filterContext);
        }
    }
}