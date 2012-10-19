namespace OutcoldSolutions.Web.Blog.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;

    using OutcoldSolutions.Web.Blog.Core;
    using OutcoldSolutions.Web.Blog.Models.Repositories;

    [HandleError]
	public class SiteController : Controller
	{
		[CustomOutputCache]
		[ValidateLang]
		public ActionResult Index(string lang)
		{
			this.Response.Cookies.Add(new HttpCookie("lang", lang) { Expires = DateTime.Now.AddYears(5) });

			using (BlogRepository repository = new BlogRepository(BlogPostLoadFlag.LoadTags | BlogPostLoadFlag.LoadAbstraction))
			{
				List<BlogPostModel> lastBlogPosts = repository.GetLast(lang, 3);
				this.ViewData["toptags"] = repository.GetTopTags(lang);
				return this.View(lastBlogPosts);
			}
		}

		[CustomOutputCache]
		[ValidateLang]
		public ActionResult Links()
		{
			return this.View();
		}

		[CustomOutputCache]
		[ValidateLang]
		public ActionResult AboutMe()
		{
			return this.View();
		}

		[CustomOutputCache]
		[ValidateLang]
		public ActionResult Search()
		{
			return this.View();
		}

		[CustomOutputCache]
		public ActionResult OpenId()
		{
			return this.View();
		}

		[OutputCache(VaryByParam = "none", Duration=0)]
		public ActionResult SiteMap()
		{
			using (BlogRepository blogController = new BlogRepository())
			{
				return this.View(blogController.GetModelForSiteMap());	
			}
		}
	}
}