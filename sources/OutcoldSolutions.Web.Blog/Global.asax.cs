// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog
{
    using System;
    using System.Diagnostics;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    using OutcoldSolutions.Web.Blog.Controllers;
    using OutcoldSolutions.Web.Blog.Services;
    using OutcoldSolutions.Web.Blog.Timers;

    using WebMatrix.WebData;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomainOnUnhandledException;

            if (Debugger.IsAttached)
            {
                Trace.Listeners.Add(new DefaultTraceListener() { TraceOutputOptions = TraceOptions.DateTime });
            }

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

                registration.Register<ISpamFilterService>()
                    .As<AkismetSpamFilterService>();

                registration.Register<ILiveJournalService>()
                    .As<LiveJournalService>();

                registration.Register<NotificationSender>();

                // Controllers
                registration.Register<AccountController>();
                registration.Register<BlogController>();
                registration.Register<CommentController>();
                registration.Register<ErrorController>();
                registration.Register<RssController>();
                registration.Register<SiteController>();
                registration.Register<StartController>();
            }

            DependencyResolver.SetResolver(container.Resolve<IDependencyResolver>());

            this.Application.Lock();
            this.Application[container.GetType().FullName] = container;
            this.Application.UnLock();
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Trace.TraceError(unhandledExceptionEventArgs.ExceptionObject.ToString());
        }
    }
}