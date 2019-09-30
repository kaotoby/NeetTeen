using BlogEngine.Core;
using BlogEngine.Core.Data;
using BlogEngine.Core.Data.Contracts;
using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Optimization;
using System.Web.UI;
using BlogEngine.NET.AppCode.Api;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;

namespace BlogEngine.NET.Custom.Themes.Standard
{
    public class ThemeConfig
    {
        private static bool _initializedAlready = false;
        private readonly static object _SyncRoot = new Object();

        public static void Initialize(HttpContext context) {
            if (_initializedAlready) { return; }

            lock (_SyncRoot) {
                if (_initializedAlready) { return; }

                RegisterBundles(BundleTable.Bundles);

                _initializedAlready = true;
            }
        }

        static void RegisterBundles(BundleCollection bundles) {
            var themeRoot = "~/Custom/Themes/Standard/src/";
            bundles.Add(new StyleBundle("~/Themes/Standard/css").Include(
                themeRoot + "css/font-face.css",
                themeRoot + "less/bootstrap.css",
                themeRoot + "css/font-awesome.css",
                themeRoot + "scss/styles.css"
                )
            );
            bundles.Add(new ScriptBundle("~/Themes/Standard/js").Include(
                themeRoot + "js/bootstrap.js",
                themeRoot + "js/custom.js"
                )
            );
        }
    }
}