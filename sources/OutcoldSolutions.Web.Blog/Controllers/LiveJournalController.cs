using System.Web.Mvc;
using PersonalWeb.Model.Repositories;

namespace PersonalWeb.Controllers
{
  public class LiveJournalController : Controller
  {
    [OutputCacheAttribute(Duration = 1800, VaryByParam = "none")]
    public ActionResult Friends()
    {
      LivejournalRepository repository = new LivejournalRepository();
      return View(repository.LoadFeeds());
    }
  }
}