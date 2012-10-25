// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using OutcoldSolutions.Web.Blog.Core.Util;

    public static class HtmlRouteData
    {
        private static readonly List<string> RusLangs;

        static HtmlRouteData()
        {
            RusLangs =
                ConfigurationUtil.GetSettings("RusLanguages", "ru, ru-ru, uk, uk-ua")
                    .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();
        }

        public static string GetLanguage(this HtmlHelper helper)
        {
            return GetLanguage(helper.ViewContext.RouteData, helper.ViewContext.RequestContext.HttpContext.Request);
        }

        public static string GetLanguage(RouteData routeData, HttpRequestBase request)
        {
            string lang = routeData.Values["lang"].To(string.Empty).ToLower();

            HttpCookie langCookie = request.Cookies["lang"];

            if (string.IsNullOrWhiteSpace(lang))
            {
                if (langCookie != null)
                {
                    lang = langCookie.Value;
                }

                if (string.IsNullOrWhiteSpace(lang))
                {
                    if (request.UserLanguages != null && request.UserLanguages.Length > 0
                        && RusLangs.Contains(request.UserLanguages[0].ToLower()))
                    {
                        lang = "ru";
                    }
                    else
                    {
                        lang = "en";
                    }
                }
            }

            if (!RusLangs.Contains(lang) && lang != "en")
            {
                lang = "en";
            }

            return lang;
        }

        public static bool IsRussian(this HtmlHelper helper)
        {
            return helper.GetLanguage() == "ru";
        }

        public static string GetAction(this HtmlHelper helper)
        {
            return helper.ViewContext.RouteData.Values["action"].To(string.Empty).ToLower();
        }

        public static string GetController(this HtmlHelper helper)
        {
            return helper.ViewContext.RouteData.Values["controller"].To(string.Empty).ToLower();
        }

        public static bool IsAction(this HtmlHelper helper, string action)
        {
            return string.Equals(helper.ViewContext.RouteData.Values["action"].ToString(), action, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsAction(this HtmlHelper helper, string action, string controller)
        {
            return helper.IsAction(action) && helper.IsController(controller);
        }

        public static bool IsController(this HtmlHelper helper, string controller)
        {
            return string.Equals(helper.ViewContext.RouteData.Values["controller"].ToString(), controller, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}