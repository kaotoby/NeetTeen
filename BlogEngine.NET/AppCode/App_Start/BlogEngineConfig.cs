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

namespace BlogEngine.NET.App_Start
{
    public class BlogEngineConfig
    {
        private static bool _initializedAlready = false;
        private readonly static object _SyncRoot = new Object();

        public static void Initialize(HttpContext context)
        {
            if (_initializedAlready) { return; }

            lock (_SyncRoot)
            {
                if (_initializedAlready) { return; }

                Utils.LoadExtensions();

                RegisterBundles(BundleTable.Bundles);

                RegisterWebApi(GlobalConfiguration.Configuration);

                RegisterDiContainer();

                ScriptManager.ScriptResourceMapping.AddDefinition("jquery",
                    new ScriptResourceDefinition
                    {
                        Path = "~/Scripts/jquery-2.1.4.min.js",
                        DebugPath = "~/Scripts/jquery-2.1.4.js",
                        CdnPath = "http://ajax.microsoft.com/ajax/jQuery/jquery-2.1.4.min.js",
                        CdnDebugPath = "http://ajax.microsoft.com/ajax/jQuery/jquery-2.1.4.js"
                    });

                _initializedAlready = true;
            }
        }

        public static void SetCulture(object sender, EventArgs e)
        {
            var culture = BlogSettings.Instance.Culture;
            if (!string.IsNullOrEmpty(culture) && !culture.Equals("Auto"))
            {
                CultureInfo defaultCulture = Utils.GetDefaultCulture();
                Thread.CurrentThread.CurrentUICulture = defaultCulture;
                Thread.CurrentThread.CurrentCulture = defaultCulture;
            }
        }

        static void RegisterBundles(BundleCollection bundles)
        {
            // for anonymous users
            bundles.Add(new StyleBundle("~/Content/Auto/css").Include(
                "~/Content/Auto/*.css")
            );
            bundles.Add(new ScriptBundle("~/Scripts/Auto/js").Include(
                "~/Scripts/Auto/*.js")
            );

            // for authenticated users
            bundles.Add(new StyleBundle("~/Content/Auto/cssauth").Include(
                "~/Content/Auto/*.css")
            );
            bundles.Add(new ScriptBundle("~/Scripts/Auto/jsauth").Include(
                "~/Scripts/Auto/*.js")
            );

            // syntax highlighter 
            var shRoot = "~/scripts/syntaxhighlighter/";
            bundles.Add(new StyleBundle("~/Content/highlighter").Include(
                shRoot + "styles/shCore.css",
                shRoot + "styles/shThemeDefault.css")
            );
            bundles.Add(new ScriptBundle("~/Scripts/highlighter").Include(
                shRoot + "scripts/XRegExp.js",
                shRoot + "scripts/shCore.js",
                shRoot + "scripts/shAutoloader.js",
                shRoot + "shActivator.js")
            );

            // new admin bundles
            bundles.IgnoreList.Clear();
            AddDefaultIgnorePatterns(bundles.IgnoreList);

            bundles.Add(
                new StyleBundle("~/Content/admincss")
                .Include("~/Content/bootstrap.css")
                .Include("~/Content/toastr.css")
                .Include("~/Content/font-awesome.min.css")
                .Include("~/Content/star-rating.css")
                .Include("~/neet/themes/standard/scss/styles.css")
                );

            bundles.Add(
                new ScriptBundle("~/scripts/blogadmin")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/jquery.form.js")
                .Include("~/Scripts/jquery.validate.js")
                .Include("~/Scripts/jquery-ui.js")
                .Include("~/Scripts/toastr.js")
                .Include("~/Scripts/bootstrap.js")
                .Include("~/Scripts/moment.js")
                .Include("~/Scripts/Q.js")
                .Include("~/Scripts/angular.min.js")
                .Include("~/Scripts/angular-route.min.js")
                .Include("~/Scripts/angular-sanitize.min.js") 
                            
                .Include("~/neet/app/app.js")
                .Include("~/neet/app/listpager.js")
                .Include("~/neet/app/grid-helpers.js")
                .Include("~/neet/app/data-service.js")
                .Include("~/neet/app/editor/filemanagerController.js")
                .Include("~/neet/app/common.js")

                .Include("~/neet/app/dashboard/dashboardController.js")

                .Include("~/neet/app/content/blogs/blogController.js")
                .Include("~/neet/app/content/posts/postController.js")
                .Include("~/neet/app/content/pages/pageController.js")
                .Include("~/neet/app/content/tags/tagController.js")
                .Include("~/neet/app/content/categories/categoryController.js")
                .Include("~/neet/app/content/comments/commentController.js")
                .Include("~/neet/app/content/comments/commentFilters.js")

                .Include("~/neet/app/custom/plugins/pluginController.js")
                .Include("~/neet/app/custom/themes/themeController.js")
                .Include("~/neet/app/custom/widgets/widgetController.js")
                .Include("~/neet/app/custom/widgets/widgetGalleryController.js")

                .Include("~/neet/app/security/users/userController.js")
                .Include("~/neet/app/security/roles/roleController.js")
                .Include("~/neet/app/security/profile/profileController.js")

                .Include("~/neet/app/settings/settingController.js")
                .Include("~/neet/app/settings/tools/toolController.js")
                .Include("~/neet/app/settings/controls/blogrollController.js")
                .Include("~/neet/app/settings/controls/pingController.js")      
                );

            bundles.Add(
                new ScriptBundle("~/scripts/wysiwyg")
                .Include("~/scripts/jquery-{version}.js")
                .Include("~/scripts/jquery.form.js")
                .Include("~/scripts/jquery.validate.js")
                .Include("~/scripts/toastr.js")
                .Include("~/scripts/Q.js")  
                .Include("~/Scripts/angular.min.js")
                .Include("~/Scripts/angular-route.min.js")
                .Include("~/Scripts/angular-sanitize.min.js")                
                .Include("~/scripts/bootstrap.js")
                .Include("~/scripts/textext.js")
                .Include("~/scripts/moment.js")
                .Include("~/neet/app/app.js")
                .Include("~/neet/app/grid-helpers.js")
                .Include("~/neet/app/editor/editor-helpers.js")
                .Include("~/neet/app/editor/posteditorController.js")
                .Include("~/neet/app/editor/pageeditorController.js")
                .Include("~/neet/app/editor/filemanagerController.js")
                .Include("~/neet/app/common.js")
                .Include("~/neet/app/data-service.js")
                );

            if (BlogConfig.DefaultEditor == "~/neet/editors/bootstrap-wysiwyg/editor.cshtml")
            {
                bundles.GetBundleFor("~/scripts/wysiwyg").Include("~/neet/editors/bootstrap-wysiwyg/jquery.hotkeys.js");
                bundles.GetBundleFor("~/scripts/wysiwyg").Include("~/neet/editors/bootstrap-wysiwyg/bootstrap-wysiwyg.js");
                bundles.GetBundleFor("~/scripts/wysiwyg").Include("~/neet/editors/bootstrap-wysiwyg/editor.js");
            }
            if (BlogConfig.DefaultEditor == "~/neet/editors/tinymce/editor.cshtml")
            {
                // tinymce plugings won't load when compressed. added in post/page editors instead.
                //bundles.GetBundleFor("~/scripts/wysiwyg").Include("~/neet/editors/tinymce/tinymce.min.js");
                //bundles.GetBundleFor("~/scripts/wysiwyg").Include("~/neet/editors/tinymce/editor.js");
            } else
            {
                bundles.GetBundleFor("~/scripts/wysiwyg").Include("~/neet/editors/summernote/summernote.js");
                // change language here if needed
                //bundles.GetBundleFor("~/scripts/wysiwyg").Include("~/neet/editors/summernote/lang/summernote-ru-RU.js");
                bundles.GetBundleFor("~/scripts/wysiwyg").Include("~/neet/editors/summernote/editor.js");
            }

#if !DEBUG
            BundleTable.EnableOptimizations = true;
#endif
        }

        static void RegisterWebApi(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute("DefaultApiWithActionAndId", "api/{controller}/{action}/{id}");

            config.Filters.Add(new UnauthorizedAccessExceptionFilterAttribute());

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Never;

            config.Services.Add(typeof(IExceptionLogger), new UnhandledExceptionLogger());
        }

        static void RegisterDiContainer()
        {
            var container = new Container();

            container.Register<ISettingsRepository, SettingsRepository>(Lifestyle.Transient);
            container.Register<IPostRepository, PostRepository>(Lifestyle.Transient);
            container.Register<IPageRepository, PageRepository>(Lifestyle.Transient);
            container.Register<IBlogRepository, BlogRepository>(Lifestyle.Transient);
            container.Register<IStatsRepository, StatsRepository>(Lifestyle.Transient);
            container.Register<IPackageRepository, PackageRepository>(Lifestyle.Transient);
            container.Register<ILookupsRepository, LookupsRepository>(Lifestyle.Transient);
            container.Register<ICommentsRepository, CommentsRepository>(Lifestyle.Transient);
            container.Register<ITrashRepository, TrashRepository>(Lifestyle.Transient);
            container.Register<ITagRepository, TagRepository>(Lifestyle.Transient);
            container.Register<ICategoryRepository, CategoryRepository>(Lifestyle.Transient);
            container.Register<ICustomFieldRepository, CustomFieldRepository>(Lifestyle.Transient);
            container.Register<IUsersRepository, UsersRepository>(Lifestyle.Transient);
            container.Register<IRolesRepository, RolesRepository>(Lifestyle.Transient);
            container.Register<IFileManagerRepository, FileManagerRepository>(Lifestyle.Transient);
            container.Register<ICommentFilterRepository, CommentFilterRepository>(Lifestyle.Transient);
            container.Register<IDashboardRepository, DashboardRepository>(Lifestyle.Transient);
            container.Register<IWidgetsRepository, WidgetsRepository>(Lifestyle.Transient);

            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);
        }

        static void AddDefaultIgnorePatterns(IgnoreList ignoreList)
        {
            if (ignoreList == null)
                throw new ArgumentNullException(nameof(ignoreList));

            ignoreList.Ignore("*.intellisense.js");
            ignoreList.Ignore("*-vsdoc.js");

            //ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
            //ignoreList.Ignore("*.min.js", OptimizationMode.WhenDisabled);
            //ignoreList.Ignore("*.min.css", OptimizationMode.WhenDisabled);
        }
    }
}