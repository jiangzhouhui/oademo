using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace OABordrinSystem.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/OpenResouce/css/bootstrap.min.css",
                      //"~/OpenResouce/font-awesome/css/font-awesome.css",
                      "~/OpenResouce/css/plugins/toastr/toastr.min.css",
                      "~/OpenResouce/css/animate.css",
                      "~/OpenResouce/css/style.css"));


            bundles.Add(new ScriptBundle("~/Script/js").Include(
                      "~/OpenResouce/js/bootstrap.min.js",
                      "~/OpenResouce/js/plugins/metisMenu/jquery.metisMenu.js",
                      "~/OpenResouce/js/plugins/slimscroll/jquery.slimscroll.min.js",
                      "~/OpenResouce/js/inspinia.js",
                      "~/OpenResouce/js/plugins/pace/pace.min.js"));

        }
    }
}