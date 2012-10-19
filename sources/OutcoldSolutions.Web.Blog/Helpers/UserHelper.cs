// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Helpers
{
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;

    using OutcoldSolutions.Web.Blog.Core.Util;

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