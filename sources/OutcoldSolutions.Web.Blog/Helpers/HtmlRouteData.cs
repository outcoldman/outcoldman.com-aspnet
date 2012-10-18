using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using PersonalWeb.Core.Util;

namespace PersonalWeb.Web.Controls
{
	public static class HtmlRouteData
	{
		private static readonly List<string> RusLangs;

		static HtmlRouteData()
		{
			RusLangs = ConfigurationUtil.GetSettings("RusLanguages", "ru, ru-ru, uk, uk-ua").Split(new[] {","},
			                                                                                    StringSplitOptions.
			                                                                                    	RemoveEmptyEntries).Select(
			                                                                                    		x => x.Trim()).ToList();
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
					lang = langCookie.Value;

				if (string.IsNullOrWhiteSpace(lang))
				{
					if (request.UserLanguages != null
						&& request.UserLanguages.Length > 0
						&& RusLangs.Contains(request.UserLanguages[0].ToLower()))
						lang = "ru";
					else
						lang = "en";
				}
			}

			if (!RusLangs.Contains(lang) && lang != "en")
				lang = "en";

			return lang;
		}

		public static bool IsRussian(this HtmlHelper helper)
		{
			return helper.GetLanguage() == "ru";
		}

		public static string GetAction(this HtmlHelper helper)
		{
			return helper.ViewContext.RequestContext.RouteData.Values["action"].To(string.Empty).ToLower();
		}

		public static string GetController(this HtmlHelper helper)
		{
			return helper.ViewContext.RequestContext.RouteData.Values["controller"].To(string.Empty).ToLower();
		}
	}
}