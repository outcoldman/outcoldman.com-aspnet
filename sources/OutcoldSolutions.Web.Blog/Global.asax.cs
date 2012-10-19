// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog
{
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    using OutcoldSolutions.Web.Blog.Core.Util;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            this.Application.Lock();
            if (ConfigurationUtil.GetSettings("SendNotifications", true))
            {
                // this.Application["notificationSender"] = new NotificationSender();
            }

            this.Application.UnLock();
        }
    }
}