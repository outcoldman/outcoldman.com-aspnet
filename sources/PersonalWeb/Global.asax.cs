using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using PersonalWeb.Core.Util;
using PersonalWeb.Web;
using PersonalWeb.Web.Timers;

namespace PersonalWeb
{
	public class MvcApplication : HttpApplication
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute("ShowIP", "ip/", new { controller = "ShowIP", action = "UserIP" });

			routes.MapRoute("SiteMap", "sitemap.xml", new { controller = "Site", action = "SiteMap" });

			routes.MapRoute("OpenID", "openid/", new { controller = "Site", action = "OpenID" });

			routes.MapRoute("Error", "Error/{action}", new { controller = "Error", action = "Unknow" });

			routes.MapRoute("Rss", "{lang}/rss/{id}", new { controller = "Rss", action = "Index", id = "" }, new { lang = new LangRouteConstraint(), id = new ExtRouteConstraint() });

			routes.MapRoute("BlogByTag", "{lang}/blog/tag/{tagid}/{id}", new { controller = "Blog", action = "Tag", id = "" }, new { lang = new LangRouteConstraint() });

			routes.MapRoute(
				"Default", // Route name
				"{lang}/{controller}/{action}/{id}", // URL with parameters
				new { controller = "start", action = "start", id = "", lang = "" } // Parameter defaults
			);

			routes.MapRoute("Catch All", "{*path}", new { controller = "Error", action = "NotFound" });
		}

		protected void Application_Start()
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			Log4NetHelper.InitLogger();

			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);

			Application.Lock();
			if (ConfigurationUtil.GetSettings("SendNotifications", true))
				Application["notificationSender"] = new NotificationSender();
			Application.UnLock();
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Log4NetHelper.Log.Fatal("UnhandledException", e.ExceptionObject as Exception);
		}
	}
}