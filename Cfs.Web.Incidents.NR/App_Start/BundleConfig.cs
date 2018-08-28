using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Cfs.Web.Incidents.NR
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts/app").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-ui.min.js",
                "~/Scripts/angular.min.js",
                "~/Scripts/bootstrap.min.js",
                "~/Scripts/ui-bootstrap.js",
                "~/Scripts/moment.min.js",
                "~/Scripts/app.js"
                ));


            bundles.Add(new StyleBundle("~/content/theme").Include(
                "~/Content/css/normalize.css",
                "~/Content/css/font-awesome.css",
                "~/Content/css/jquery-ui.min.css",
                "~/Content/css/bootstrap.css",
                "~/Content/css/bootstrap-theme.css",
                "~/Content/css/default.css"
                ));
        }
    }
}