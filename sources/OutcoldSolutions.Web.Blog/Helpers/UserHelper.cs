using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using PersonalWeb.Core.Util;

namespace PersonalWeb.Web.Controls
{
	public static class UserHelper
	{
		public static bool IsMe(this HtmlHelper helper)
		{
			return helper.ViewContext.HttpContext.IsMe();
		}

		public static bool IsMe(this HttpContextBase context)
		{
			IPrincipal principal = context.User;
			return principal != null && principal.Identity != null && principal.Identity.Name == ConfigurationUtil.Me;
		}
	}
}