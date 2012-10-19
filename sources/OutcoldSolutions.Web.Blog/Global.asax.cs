// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog
{
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    using OutcoldSolutions.Web.Blog.Controllers;

    using WebMatrix.WebData;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            WebSecurity.InitializeDatabaseConnection("LocalDatabase", "UserProfile", "UserId", "UserName", autoCreateTables: false);

            IDependencyResolverContainer container = new DependencyResolverContainer();
            using (var registration = container.Registration())
            {
                registration.Register<IDependencyResolver>()
                    .AsSingleton<CustomDependencyResolver>();
                registration.Register<AccountController>();
                registration.Register<BlogController>();
                registration.Register<CommentController>();
                registration.Register<ErrorController>();
                registration.Register<LiveJournalController>();
                registration.Register<RssController>();
                registration.Register<SiteController>();
                registration.Register<StartController>();
            }

            DependencyResolver.SetResolver(container.Resolve<IDependencyResolver>());

            this.Application.Lock();
            this.Application[container.GetType().FullName] = container;

            //if (ConfigurationUtil.GetSettings("SendNotifications", true))
            //{
                // this.Application["notificationSender"] = new NotificationSender();
            //}

            this.Application.UnLock();
        }
    }
}