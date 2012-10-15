using System.Web;
using System.Web.Routing;
using PersonalWeb.Core.Util;

namespace PersonalWeb.Web
{
	public class ExtRouteConstraint : IRouteConstraint
	{
		public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values,
		                  RouteDirection routeDirection)
		{
			if ((routeDirection == RouteDirection.IncomingRequest) && (parameterName.ToLower() == "id"))
			{
				string lang = values["id"].To(string.Empty).ToLower();
				if (string.IsNullOrEmpty(lang) || lang == "ext")
					return true;
			}
			return false;
		}
	}
}