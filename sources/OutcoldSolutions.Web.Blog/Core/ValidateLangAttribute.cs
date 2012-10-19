// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Core
{
    using System.Web;
    using System.Web.Mvc;

    using OutcoldSolutions.Web.Blog.Core.Util;

    public class ValidateLangAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.RouteData.Values.ContainsKey("lang"))
            {
                string lang = context.RouteData.Values["lang"].To(string.Empty);
                if (lang == "en" || lang == "ru"
                    || (string.IsNullOrEmpty(lang)
                        && context.RouteData.Values["action"].To(string.Empty).ToLower() == "start"
                        && context.RouteData.Values["controller"].To(string.Empty).ToLower() == "start"))
                {
                    return;
                }
            }

            throw new HttpException(404, "Couldn't find language");
        }
    }
}