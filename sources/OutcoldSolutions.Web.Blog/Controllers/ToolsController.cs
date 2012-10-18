using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using CodeSnippet.Config;
using CodeSnippet.Formats;
using CodeSnippet.Formats.Base;
using PersonalWeb.Core.Util;
using PersonalWeb.Web;
using PersonalWeb.Web.Resources;

namespace PersonalWeb.Controllers
{
    using System.Diagnostics;

    public class ToolsController : Controller
	{
		#region RegEx

		[HttpGet]
		[ValidateLang]
		public ActionResult RegEx(string lang)
		{
			return View();
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
					matches.AppendFormat(ResourceLoader.GetResource(lang, "MatchResult"), (i + 1), match.Index, Server.HtmlEncode(matchCollection[i].Value));
				}
				res = true;
			}
			catch (Exception e)
			{
				errorinfo = e.Message;
				res = false;
			}

			return Json(new { res, matches = matches.ToString(), errorinfo });
		}

		#endregion

		#region CodeHighlighter

		[HttpGet]
		[ValidateLang]
		public ActionResult CodeHighlighter(string lang)
		{
			var service = new SourceCodeHighlighterService();
			List<string> supportedFormats = service.GetSupportedFormats();
			ViewBag.SupportedFormats = supportedFormats.Select(x => new SelectListItem() { Text = x, Value = x, Selected = x == "cs" }).ToList();
			return View();
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

			return Json(new { res, formattedCode = highlightedCode, formattedCodeEscaped = Server.HtmlEncode(highlightedCode) });
		}

		#endregion


		[HttpGet]
		[ValidateLang]
		public ActionResult Index(string lang)
		{
			return View();
		}

		[HttpGet]
		[ValidateLang]
		public ActionResult TwitterStatistic(string lang)
		{
			return View();
		}

		[HttpGet]
		[ValidateLang]
		public ActionResult Keys(string lang)
		{
			return View("KeysExtender");
		}
	}
}