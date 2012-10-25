// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Helpers
{
    using System.Security.Principal;
    using System.Web;

    using OutcoldSolutions.Web.Blog.Core.Util;

    public static class UserHelper
    {
        public static bool IsMe(this HttpContextBase context)
        {
            IPrincipal principal = context.User;
            return principal != null && principal.Identity != null && principal.Identity.Name == ConfigurationUtil.AuthorUsername;
        }
    }
}