using System.Web.Mvc;
using PersonalWeb.Core.Util;

namespace PersonalWeb.Web
{
	public class MeAuthorizeAttribute : AuthorizeAttribute
	{
		public MeAuthorizeAttribute()
		{
			Users = ConfigurationUtil.Me;
		}
	}
}