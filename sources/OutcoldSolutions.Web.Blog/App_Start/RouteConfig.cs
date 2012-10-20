// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog
{
    using System.Web.Mvc;
    using System.Web.Routing;

    using OutcoldSolutions.Web.Blog.Core;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Account", "account/login", new { controller = "Account", action = "Login" });

            routes.MapRoute("ShowIP", "ip/", new { controller = "ShowIP", action = "UserIP" });

            routes.MapRoute("SiteMap", "sitemap.xml", new { controller = "Site", action = "SiteMap" });

            routes.MapRoute("OpenID", "openid/", new { controller = "Site", action = "OpenID" });

            routes.MapRoute("Error", "Error/{action}", new { controller = "Error", action = "Unknow" });

            routes.MapRoute("Rss", "{lang}/rss", new { controller = "Rss", action = "Index" }, new { lang = new LangRouteConstraint() });

            routes.MapRoute("BlogByTag", "{lang}/blog/tag/{tagid}/{id}", new { controller = "Blog", action = "Tag", id = "" }, new { lang = new LangRouteConstraint() });

            routes.MapRoute(
                "Default", // Route name
                "{lang}/{controller}/{action}/{id}", // URL with parameters
                new { controller = "start", action = "start", id = "", lang = "" } // Parameter defaults
            );

            routes.MapRoute("Catch All", "{*path}", new { controller = "Error", action = "NotFound" });
        }
    }
}