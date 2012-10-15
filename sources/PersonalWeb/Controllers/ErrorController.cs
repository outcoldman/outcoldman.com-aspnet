using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PersonalWeb.Core.Util;

namespace PersonalWeb.Controllers
{
    public class ErrorController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Unknow()
        {
			LogError();
            ViewData["ErrorMessage"] = "Unexpected error.";
            return View("Error");
        }

    	[AcceptVerbs(HttpVerbs.Get)]
        public ActionResult NotFound(string aspxerrorpath)
        {
			LogError();
            ViewData["ErrorMessage"] = string.Format("Error 404. Couldn't find page with current url {0}. Sorry.", aspxerrorpath);
            return View("Error");
        }

		private void LogError()
		{
			if (HttpContext.Error != null)
				Log4NetHelper.Log.Error("Unknow", HttpContext.Error);
		}
    }
}
