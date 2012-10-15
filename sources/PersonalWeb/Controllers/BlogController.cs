using System;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using PersonalWeb.Core.Util;
using PersonalWeb.Model;
using PersonalWeb.Model.Repositories;
using PersonalWeb.Web;
using PersonalWeb.Web.Controls;
using PersonalWeb.Web.Resources;

namespace PersonalWeb.Controllers
{
	public class BlogController : Controller
	{
		[HttpGet]
		[CustomOutputCache]
		[ValidateLang]
		public ActionResult Index(string lang, int? id)
		{
			using (
				BlogRepository repository = new BlogRepository(BlogPostLoadFlag.LoadAbstraction | BlogPostLoadFlag.LoadTags))
			{
				int pageIndex = id ?? 1;
				ViewData["selectedPage"] = pageIndex;

				int postsCount = repository.GetPostsCount(lang);
				ViewData["pagesCount"] = postsCount/PagesControl.ItemsPerPage +
				                         (postsCount%PagesControl.ItemsPerPage == 0 ? 0 : 1);
				return View("Index", repository.GetPosts(pageIndex, PagesControl.ItemsPerPage, lang));
			}
		}

		[HttpGet]
		[CustomOutputCache]
		[ValidateLang]
		public ActionResult Tag(string lang, int tagid, int? id)
		{
			using (
				BlogRepository repository = new BlogRepository(BlogPostLoadFlag.LoadAbstraction | BlogPostLoadFlag.LoadTags))
			{
				int pageIndex = id ?? 1;
				ViewData["selectedPage"] = pageIndex;
				Tag tag = repository.GetTag(tagid);

				if (tag == null)
				{
					throw new HttpException((int) HttpStatusCode.NotFound,
					                        string.Format("Couldn't find tag with ID {0}", id));
				}

				ViewData["tag"] = tag;
				int postsCount = repository.GetPostsByTagCount(lang, tagid);
				ViewData["pagesCount"] = postsCount/PagesControl.ItemsPerPage +
				                         (postsCount%PagesControl.ItemsPerPage == 0 ? 0 : 1);
				
				return View("Index", repository.GetPostsByTag(pageIndex, PagesControl.ItemsPerPage, lang, tagid));
			}
		}

		[HttpGet]
		[CustomOutputCache]
		[ValidateLang]
		public ActionResult Show(string lang, int id)
		{
			using (BlogRepository blogRepository = new BlogRepository(BlogPostLoadFlag.LoadBody | BlogPostLoadFlag.LoadTags))
			{
				BlogPost blogPost = blogRepository.LoadPost(id);
				CheckBlogPostExist(id, lang, blogPost);
				ViewData["comments"] = blogRepository.GetComments(id);
				// For master page
				ViewData["keywords"] = blogPost.TagsLine;
				ViewData["description"] = Server.HtmlEncode(blogPost.Title);
				// ------
				ViewData["simpleposts"] = blogRepository.GetLikePosts(id);
				return View("ItemView", blogPost);
			}
		}

		[HttpGet]
		[MeAuthorizeAttribute]
		public ActionResult ItemEdit(string lang, int id)
		{
			using (BlogRepository blogRepository = new BlogRepository(BlogPostLoadFlag.FullLoad))
			{
				BlogPost blogPost = blogRepository.LoadPost(id);
				CheckBlogPostExist(id, lang, blogPost);
				return View("ItemEdit", blogPost);
			}
		}

		[HttpPost]
		[ValidateInput(false)]
		[MeAuthorizeAttribute]
		public ActionResult ItemEdit(string lang, int id, FormCollection formValues)
		{
			using (BlogRepository blogRepository = new BlogRepository(BlogPostLoadFlag.FullLoad))
			{
				BlogPost blogPost = blogRepository.LoadPost(id);
				CheckBlogPostExist(id, lang, blogPost);

				ValidateBlogPost(blogPost, formValues);

				if (ModelState.IsValid)
				{
					UpdateModel(blogPost);
					blogRepository.Save(blogPost, formValues["TagsLine"]);
					return RedirectToAction("show", "blog", new {id, lang = blogPost.Language});
				}

				return View(blogPost);
			}
		}

		[HttpGet]
		[MeAuthorizeAttribute]
		public ActionResult Create(string lang)
		{
			BlogPost blogPost = new BlogPost {Date = DateTime.Now.ToUniversalTime()};
			return View("ItemEdit", blogPost);
		}

		[HttpPost]
		[ValidateInput(false)]
		[MeAuthorizeAttribute]
		public ActionResult Create(string lang, FormCollection formValues)
		{
			using (BlogRepository blogRepository = new BlogRepository())
			{
				BlogPost blogPost = new BlogPost();

				ValidateBlogPost(blogPost, formValues);

				if (ModelState.IsValid)
				{
					UpdateModel(blogPost);
					blogRepository.Save(blogPost, formValues["TagsLine"]);
					return RedirectToAction("show", "blog", new {id = blogPost.PostID, lang = blogPost.Language});
				}

				return View("ItemEdit", blogPost);
			}
		}

		[HttpGet]
		[MeAuthorizeAttribute]
		public ActionResult List(string lang, string action, string controller, int? id)
		{
			using (BlogRepository repository = new BlogRepository())
			{
				int pageIndex = id ?? 1;
				ViewData["selectedPage"] = pageIndex;
				int postsCount = repository.GetPostsCount();
				ViewData["pagesCount"] = postsCount/PagesControl.ItemsPerPage +
				                         (postsCount%PagesControl.ItemsPerPage == 0 ? 0 : 1);
				return View("List", repository.GetPosts(pageIndex, PagesControl.ItemsPerPage));
			}
		}

		#region Delete post

		[HttpGet]
		[MeAuthorizeAttribute]
		public ActionResult Delete(string lang, int id)
		{
			using (BlogRepository repository = new BlogRepository())
			{
				BlogPost blogPost = repository.LoadPost(id);
				CheckBlogPostExist(id, lang, blogPost);
				return View("Delete", blogPost);
			}
		}

		[HttpPost]
		[MeAuthorizeAttribute]
		public ActionResult Delete(string lang, int id, string confirmButton)
		{
			using (BlogRepository repository = new BlogRepository())
			{
				BlogPost blogPost = repository.LoadPost(id);
				CheckBlogPostExist(id, lang, blogPost);
				repository.DataContext.BlogPosts.DeleteObject(blogPost);
				repository.DataContext.SaveChanges();
				return RedirectToAction("index");
			}
		}

		#endregion

		[HttpGet]
		[MeAuthorizeAttribute]
		public ActionResult Publicate(string lang, int id)
		{
			try
			{
				using (BlogRepository repository = new BlogRepository())
				{
					BlogPost blogPost = repository.LoadPost(id);
					using (SmtpClient smtpClient = SmtpConfig.GetClient())
					{
						MailMessage message = new MailMessage(SmtpConfig.GetFrom(), ConfigurationUtil.LivejournalEmail)
						                      	{
						                      		Subject = blogPost.Title,
						                      		Body =
						                      			string.Format("lj-tags: {0} {1} <lj-raw>{2} {3}</lj-raw>", blogPost.TagsLine,
						                      			              Environment.NewLine,
						                      			              blogPost.HtmlAbstraction,
						                      			              string.Format("<p><a href=\"{0}\">{1}</a></p>",
						                      			                            NavigationHelper.GetUrlWithHost(
						                      			                            	Url.Action("show", "blog",
						                      			                            	           new RouteValueDictionary
						                      			                            	           	{
						                      			                            	           		{"lang", blogPost.Language},
						                      			                            	           		{"id", blogPost.PostID}
						                      			                            	           	})),
						                      			                            ResourceLoader.GetResource(lang,
						                      			                                                       "ReadMoreRss")))
						                      	};
						smtpClient.Send(message);
					}
				}
			}
			catch (Exception e)
			{
				Log4NetHelper.Log.Error(e);
			}
			return RedirectToAction("show", new {id, lang});
		}

		private void ValidateBlogPost(BlogPost blogPost, FormCollection formValues)
		{
			blogPost.Title = formValues["Title"];
			if (string.IsNullOrEmpty(blogPost.Title))
				AddCannotEmptyError("Title", "Title");

			string strDate = formValues["Date"];
			DateTime date;
			if (DateTime.TryParse(strDate, out date))
				blogPost.Date = date;
			else
				ModelState.AddModelError("Date", @"Check that date in correct format.");

			blogPost.HtmlAbstraction = formValues["HtmlAbstraction"];
			if (string.IsNullOrEmpty(blogPost.HtmlAbstraction))
				AddCannotEmptyError("HtmlAbstraction", "Abstraction");

			blogPost.HtmlText = formValues["HtmlText"];
			if (string.IsNullOrEmpty(blogPost.HtmlText))
				AddCannotEmptyError("HtmlText", "Body cannot be empty");
		}

		private void AddCannotEmptyError(string field, string name)
		{
			ModelState.AddModelError(field, string.Format("{0} cannot be empty", name));
		}

		private void CheckBlogPostExist(int id, string lang, BlogPost blogPost)
		{
			if (blogPost == null || blogPost.PostID != id || string.Compare(blogPost.Language, lang, true) != 0
				|| ((!blogPost.IsPublic || blogPost.Date > DateTime.UtcNow) && !HttpContext.IsMe()))
				throw new HttpException((int) HttpStatusCode.NotFound,
				                        string.Format("Couldn't find blog post with ID {0}", id));
		}
	}
}