using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.Security.Application;
using PersonalWeb.Core;
using PersonalWeb.Core.Util;
using PersonalWeb.Model;
using PersonalWeb.Model.Repositories;
using PersonalWeb.Web;
using PersonalWeb.Web.Controls;
using PersonalWeb.Web.Resources;

namespace PersonalWeb.Controllers
{
    using System.Diagnostics;

    using OutcoldSolutions.Web.Blog.Models;

    public class CommentController : Controller
	{
		[ValidateInput(false)]
		[HttpPost]
		[ValidateLang]
		public ActionResult Add(int id, string body, string name, string email, bool fInform, string lang, string site)
		{
			var userIP = Request.UserHostAddress;
			var userAgent = Request.UserAgent;
			bool result = false;
			body = Sanitizer.GetSafeHtmlFragment(HtmlParser.DoBr(HtmlParser.HighlightCode(body).Trim()));
			site = Sanitizer.GetSafeHtmlFragment(site);
			string commentView = string.Empty;

			string resMessage = ResourceLoader.GetResource(lang, "CommentNotAdded");

			if (!string.IsNullOrEmpty(body))
			{
				try
				{
					using (BlogRepository repository = new BlogRepository())
					{
						BlogPost blogPost = repository.LoadPost(id);
						if (blogPost != null)
						{
							Comment comment = new Comment
							                  	{
							                  		PostID = id,
							                  		Body = body,
							                  		Date = DateTime.Now.ToUniversalTime(),
							                  		UserEmail = Server.HtmlEncode(email.Trim()).SafelySubstring(100),
							                  		UserName = Server.HtmlEncode(name.Trim()).SafelySubstring(100),
							                  		UserIP = userIP,
							                  		UserWeb = Server.UrlDecode(HtmlParser.AddHttp(site.Trim())).SafelySubstring(100),
							                  		UserAgent = userAgent,
							                  		IsSpam = false
							                  	};

							if (repository.CheckComment(comment))
							{
								AkismetRepository akismetRepository = new AkismetRepository();
								comment.IsSpam = akismetRepository.IsSpam(comment);

								CommentSubscription subscription = null;
								if (fInform && email != ConfigurationUtil.MeEmail)
								{
									if (!repository.CheckSubscriptionExist(id, email))
										subscription = new CommentSubscription
										               	{
										               		PostID = id,
										               		Email = comment.UserEmail,
										               		SubscriptionID = Guid.NewGuid()
										               	};
								}

								repository.AddComment(comment, subscription);
								result = true;
								commentView = this.RenderViewToString("CommentViewItem", comment);

								resMessage = comment.IsSpam
								             	? ResourceLoader.GetResource(lang, "CommentAddedAsSpam")
								             	: ResourceLoader.GetResource(lang, "CommentAdded");
							}
							else
							{
								resMessage = ResourceLoader.GetResource(lang, "DublicateComment");
							}
						}
					}
				}
				catch (Exception e)
				{
                    Trace.TraceError(e.ToString());
				}
			}

			return Json(new {resMessage, result, commentView});
		}

		[ValidateInput(false)]
		[HttpPost]
		public ActionResult Preview(int id, string body, string name, string site, string email)
		{
			body = Sanitizer.GetSafeHtmlFragment(HtmlParser.DoBr(HtmlParser.HighlightCode(body).Trim()));
			site = Sanitizer.GetSafeHtmlFragment(site);

			Comment comment = new Comment
			{
				PostID = id,
				Body = body,
				Date = DateTime.Now.ToUniversalTime(),
				UserName = Server.HtmlEncode(name.Trim()).SafelySubstring(100),
				UserWeb = Server.UrlDecode(HtmlParser.AddHttp(site.Trim())).SafelySubstring(100),
				IsSpam = false,
				UserEmail = Server.HtmlEncode(email.Trim()).SafelySubstring(100)
			};

			var commentView = this.RenderViewToString("CommentViewItem", comment);

			return Json(new { commentView });
		}

		[HttpGet]
		[ValidateLang]
		public ActionResult Unsubscribe(string lang, Guid? id)
		{
			if (!id.HasValue)
				throw new HttpException(404, "Couldn't find current unsubscribe");

			using (BlogRepository blogRepository = new BlogRepository())
			{
				UnsubscribeModel unsubscribeModel = blogRepository.LoadUnsubscribeModel(id.Value);
				if (unsubscribeModel == null)
				{
					throw new HttpException(404, string.Format("Couldn't find current unsubscribe '{0}'", id));
				}
				return View("Unsubscribe", unsubscribeModel);
			}
		}

		[HttpPost]
		[ValidateLang]
		public ActionResult Unsubscribe(string lang, Guid id, FormCollection collection)
		{
			using (BlogRepository blogRepository = new BlogRepository())
			{
				CommentSubscription subscription = blogRepository.LoadSubscription(id);
				if (subscription == null)
				{
					throw new HttpException(404, string.Format("Couldn't fing current unsubscribe '{0}'", id));
				}
				int postId = subscription.PostID;
				blogRepository.DeleteSubscribtion(subscription);
				return RedirectToAction("show", "blog", new {id = postId, lang});
			}
		}

		[HttpGet]
		[MeAuthorizeAttribute]
		public ActionResult Delete(string lang, int id)
		{
			using (BlogRepository repository = new BlogRepository())
			{
				Comment comment = repository.LoadComment(id);
				if (comment == null)
					throw new HttpException(404, string.Format("Couldn't find comment with ID '{0}'", id));
				return View("Delete", comment);
			}
		}

		[HttpPost]
		[MeAuthorizeAttribute]
		public ActionResult Delete(string lang, int id, string confirmButton)
		{
			using (BlogRepository repository = new BlogRepository())
			{
				Comment comment = repository.LoadComment(id);
				if (comment == null)
					throw new HttpException(404, string.Format("Couldn't find comment with ID '{0}'", id));
				int postId = comment.PostID;
				repository.DataContext.Comments.DeleteObject(comment);
				repository.DataContext.SaveChanges();
				return RedirectToAction("show", "blog", new {id = postId});
			}
		}

		[HttpGet]
		[MeAuthorizeAttribute]
		public ActionResult Spam(string lang, int id)
		{
			using (BlogRepository repository = new BlogRepository())
			{
				Comment comment = repository.LoadComment(id);
				if (comment == null)
					throw new HttpException(404, string.Format("Couldn't find comment with ID '{0}'", id));
				comment.IsSpam = true;
				AkismetRepository akismetRepository = new AkismetRepository();
				akismetRepository.SubmitSpam(comment);
				repository.DataContext.SaveChanges();
				return RedirectToAction("list");
			}
		}

		[HttpGet]
		[MeAuthorizeAttribute]
		public ActionResult UnSpam(string lang, int id)
		{
			using (BlogRepository repository = new BlogRepository())
			{
				Comment comment = repository.LoadComment(id);
				if (comment == null)
					throw new HttpException(404, string.Format("Couldn't find comment with ID '{0}'", id));
				comment.IsSpam = false;
				AkismetRepository akismetRepository = new AkismetRepository();
				akismetRepository.SubmitUnSpam(comment);
				repository.DataContext.SaveChanges();
				repository.SetNotifications(comment);
				return RedirectToAction("list");
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
				int postsCount = repository.GetCommentsCount();
				ViewData["pagesCount"] = postsCount/PagesControl.ItemsPerPage + (postsCount%PagesControl.ItemsPerPage == 0 ? 0 : 1);
				return View("List", repository.GetComments(pageIndex, PagesControl.ItemsPerPage));
			}
		}
	}
}