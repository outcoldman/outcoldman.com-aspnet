// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Controllers
{
    using System.Web.Mvc;

    using OutcoldSolutions.Web.Blog.Models.Repositories;

    public class LiveJournalController : Controller
    {
        [OutputCache(Duration = 1800, VaryByParam = "none")]
        public ActionResult Friends()
        {
            LivejournalRepository repository = new LivejournalRepository();
            return this.View(repository.LoadFeeds());
        }
    }
}