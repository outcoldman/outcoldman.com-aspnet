// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Core
{
    using System.Web.Mvc;

    using OutcoldSolutions.Web.Blog.Core.Util;

    public class CustomOutputCacheAttribute : OutputCacheAttribute
    {
        public CustomOutputCacheAttribute()
        {
            this.Duration = ConfigurationUtil.DefaultCacheValue;
            this.VaryByParam = "none";
        }
    }
}