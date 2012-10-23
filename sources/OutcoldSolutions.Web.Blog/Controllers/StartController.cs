// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Controllers
{
    using System.Web.Mvc;

    using OutcoldSolutions.Web.Blog.Helpers;

    public class StartController : Controller
    {
        public ActionResult Start()
        {
            string lang = HtmlRouteData.GetLanguage(this.RouteData, this.Request);
            return this.RedirectToAction("index", "blog", new { lang });
        }
    }
}