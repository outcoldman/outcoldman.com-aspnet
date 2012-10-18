using System.Web.Mvc;
using PersonalWeb.Web.Controls;

namespace PersonalWeb.Controllers
{
	public class StartController : Controller
	{
		public ActionResult Start()
		{
			string lang = HtmlRouteData.GetLanguage(RouteData, Request);
			return RedirectToAction("index", "site", new {lang});
		}
	}
}