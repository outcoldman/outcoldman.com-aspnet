// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Controllers
{
    using System.Web.Mvc;

    using OutcoldSolutions.Web.Blog.Core;
    using OutcoldSolutions.Web.Blog.Models.Repositories;

    [HandleError]
    public class SiteController : Controller
    {
        [CustomOutputCache]
        [ValidateLang]
        public ActionResult AboutMe()
        {
            return this.View();
        }

        [CustomOutputCache]
        public ActionResult OpenId()
        {
            return this.View();
        }

        [OutputCache(VaryByParam = "none", Duration = 0)]
        public ActionResult SiteMap()
        {
            using (BlogRepository blogController = new BlogRepository())
            {
                return this.View(blogController.GetModelForSiteMap());
            }
        }
    }
}