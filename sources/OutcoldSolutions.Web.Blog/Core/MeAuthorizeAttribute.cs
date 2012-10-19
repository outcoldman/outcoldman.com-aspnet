// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Core
{
    using System.Web.Mvc;

    using OutcoldSolutions.Web.Blog.Core.Util;

    public class MeAuthorizeAttribute : AuthorizeAttribute
    {
        public MeAuthorizeAttribute()
        {
            this.Users = ConfigurationUtil.Me;
        }
    }
}