using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using PersonalWeb.Model.Repositories;
using PersonalWeb.Web;

namespace PersonalWeb.Controllers
{
	[HandleError]
	public class SiteController : Controller
	{
		[CustomOutputCache]
		[ValidateLang]
		public ActionResult Index(string lang)
		{
			Response.Cookies.Add(new HttpCookie("lang", lang) { Expires = DateTime.Now.AddYears(5) });

			using (BlogRepository repository = new BlogRepository(BlogPostLoadFlag.LoadTags | BlogPostLoadFlag.LoadAbstraction))
			{
				List<BlogPostModel> lastBlogPosts = repository.GetLast(lang, 3);
				ViewData["toptags"] = repository.GetTopTags(lang);
				return View(lastBlogPosts);
			}
		}

		[CustomOutputCache]
		[ValidateLang]
		public ActionResult Links()
		{
			return View();
		}

		[CustomOutputCache]
		[ValidateLang]
		public ActionResult AboutMe()
		{
			return View();
		}

		[CustomOutputCache]
		[ValidateLang]
		public ActionResult Search()
		{
			return View();
		}

		[CustomOutputCache]
		public ActionResult OpenId()
		{
			return View();
		}

		[OutputCache(VaryByParam = "none", Duration=0)]
		public ActionResult SiteMap()
		{
			using (BlogRepository blogController = new BlogRepository())
			{
				return View(blogController.GetModelForSiteMap());	
			}
		}
	}
}