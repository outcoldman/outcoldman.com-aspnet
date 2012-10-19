// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Core
{
    using System.Web;
    using System.Web.Routing;

    using OutcoldSolutions.Web.Blog.Core.Util;

    public class LangRouteConstraint : IRouteConstraint
    {
        public bool Match(
            HttpContextBase httpContext, 
            Route route, 
            string parameterName, 
            RouteValueDictionary values, 
            RouteDirection routeDirection)
        {
            if ((routeDirection == RouteDirection.IncomingRequest) && (parameterName.ToLower() == "lang"))
            {
                string lang = values["lang"].To(string.Empty).ToLower();
                if (lang == "en" || lang == "ru"
                    || (string.IsNullOrEmpty(lang) && values["action"].To(string.Empty).ToLower() == "start"
                        && values["controller"].To(string.Empty).ToLower() == "start"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}