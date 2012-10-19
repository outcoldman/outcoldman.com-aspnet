﻿// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Controllers
{
    using System.Diagnostics;
    using System.Web.Mvc;

    public class ErrorController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Unknow()
        {
            this.LogError();
            this.ViewData["ErrorMessage"] = "Unexpected error.";
            return this.View("Error");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult NotFound(string aspxerrorpath)
        {
            this.LogError();
            this.ViewData["ErrorMessage"] = string.Format(
                "Error 404. Couldn't find page with current url {0}. Sorry.", aspxerrorpath);
            return this.View("Error");
        }

        private void LogError()
        {
            if (this.HttpContext.Error != null)
            {
                Trace.TraceError(this.HttpContext.Error.ToString());
            }
        }
    }
}