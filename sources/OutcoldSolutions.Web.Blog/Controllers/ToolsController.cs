// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;

    using OutcoldSolutions.Web.Blog.Core;
    using OutcoldSolutions.Web.Blog.Resources;

    public class ToolsController : Controller
    {
        [HttpGet]
        [ValidateLang]
        public ActionResult RegEx(string lang)
        {
            return this.View();
        }

        [ValidateInput(false)]
        [HttpPost]
        [ValidateLang]
        public ActionResult RegEx(string lang, string regex, string checkstring)
        {
            bool res;
            string errorinfo = string.Empty;
            StringBuilder matches = new StringBuilder();
            try
            {
                MatchCollection matchCollection = Regex.Matches(checkstring, regex);
                for (int i = 0; i < matchCollection.Count; i++)
                {
                    Match match = matchCollection[i];
                    matches.AppendFormat(
                        ResourceLoader.GetResource(lang, "MatchResult"), 
                        i + 1, 
                        match.Index, 
                        this.Server.HtmlEncode(matchCollection[i].Value));
                }

                res = true;
            }
            catch (Exception e)
            {
                errorinfo = e.Message;
                res = false;
            }

            return this.Json(new { res, matches = matches.ToString(), errorinfo });
        }

        [HttpGet]
        [ValidateLang]
        public ActionResult CodeHighlighter(string lang)
        {
            var service = new SourceCodeHighlighterService();
            List<string> supportedFormats = service.GetSupportedFormats();
            this.ViewBag.SupportedFormats =
                supportedFormats.Select(x => new SelectListItem() { Text = x, Value = x, Selected = x == "cs" })
                    .ToList();
            return this.View();
        }

        [ValidateInput(false)]
        [HttpPost]
        [ValidateLang]
        public ActionResult CodeHighlighter(string lang, string inputcode, string type)
        {
            bool res = true;
            string highlightedCode = string.Empty;

            try
            {
                var service = new SourceCodeHighlighterService();
                highlightedCode = service.HighlightCode(inputcode, type);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                res = false;
            }

            return
                this.Json(
                    new
                        {
                            res, 
                            formattedCode = highlightedCode, 
                            formattedCodeEscaped = this.Server.HtmlEncode(highlightedCode)
                        });
        }

        [HttpGet]
        [ValidateLang]
        public ActionResult Index(string lang)
        {
            return this.View();
        }

        [HttpGet]
        [ValidateLang]
        public ActionResult TwitterStatistic(string lang)
        {
            return this.View();
        }

        [HttpGet]
        [ValidateLang]
        public ActionResult Keys(string lang)
        {
            return this.View("KeysExtender");
        }
    }
}