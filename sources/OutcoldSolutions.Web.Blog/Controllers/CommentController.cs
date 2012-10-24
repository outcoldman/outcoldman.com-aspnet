// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Web;
    using System.Web.Mvc;

    using Microsoft.Security.Application;

    using OutcoldSolutions.Web.Blog.Core;
    using OutcoldSolutions.Web.Blog.Core.Util;
    using OutcoldSolutions.Web.Blog.Helpers;
    using OutcoldSolutions.Web.Blog.Models;
    using OutcoldSolutions.Web.Blog.Models.Repositories;
    using OutcoldSolutions.Web.Blog.Resources;
    using OutcoldSolutions.Web.Blog.Services;

    public class CommentController : Controller
    {
        private readonly ISpamFilterService spamFilterService;

        public CommentController(ISpamFilterService spamFilterService)
        {
            this.spamFilterService = spamFilterService;
        }

        [ValidateInput(false)]
        [HttpPost]
        [ValidateLang]
        public ActionResult Add(int id, string body, string name, string email, bool fInform, string lang, string site)
        {
            var userIP = this.Request.UserHostAddress;
            var userAgent = this.Request.UserAgent;
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
                                    UserEmail =
                                        this.Server.HtmlEncode(email.Trim())
                                        .SafelySubstring(100), 
                                    UserName =
                                        this.Server.HtmlEncode(name.Trim()).SafelySubstring(100), 
                                    UserIP = userIP, 
                                    UserWeb =
                                        this.Server.UrlDecode(HtmlParser.AddHttp(site.Trim()))
                                        .SafelySubstring(100), 
                                    UserAgent = userAgent, 
                                    IsSpam = false
                                };

                            if (repository.CheckComment(comment))
                            {
                                comment.IsSpam = this.spamFilterService.IsSpam(
                                    comment.Body,
                                    comment.UserIP,
                                    comment.UserAgent,
                                    comment.UserName,
                                    comment.UserEmail,
                                    comment.UserWeb);

                                CommentSubscription subscription = null;
                                if (fInform && email != ConfigurationUtil.AuthorEmail)
                                {
                                    if (!repository.CheckSubscriptionExist(id, email))
                                    {
                                        subscription = new CommentSubscription
                                            {
                                                PostID = id, 
                                                Email = comment.UserEmail, 
                                                SubscriptionID = Guid.NewGuid()
                                            };
                                    }
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

            return this.Json(new { resMessage, result, commentView });
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
                    UserName = this.Server.HtmlEncode(name.Trim()).SafelySubstring(100), 
                    UserWeb =
                        this.Server.UrlDecode(HtmlParser.AddHttp(site.Trim()))
                        .SafelySubstring(100), 
                    IsSpam = false, 
                    UserEmail = this.Server.HtmlEncode(email.Trim()).SafelySubstring(100)
                };

            var commentView = this.RenderViewToString("CommentViewItem", comment);

            return this.Json(new { commentView });
        }

        [HttpGet]
        [ValidateLang]
        public ActionResult Unsubscribe(string lang, Guid? id)
        {
            if (!id.HasValue)
            {
                throw new HttpException(404, "Couldn't find current unsubscribe");
            }

            using (BlogRepository blogRepository = new BlogRepository())
            {
                UnsubscribeModel unsubscribeModel = blogRepository.LoadUnsubscribeModel(id.Value);
                if (unsubscribeModel == null)
                {
                    throw new HttpException(404, string.Format("Couldn't find current unsubscribe '{0}'", id));
                }

                return this.View("Unsubscribe", unsubscribeModel);
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
                return this.RedirectToAction("show", "blog", new { id = postId, lang });
            }
        }

        [HttpGet]
        [MeAuthorize]
        public ActionResult Delete(string lang, int id)
        {
            using (BlogRepository repository = new BlogRepository())
            {
                Comment comment = repository.LoadComment(id);
                if (comment == null)
                {
                    throw new HttpException(404, string.Format("Couldn't find comment with ID '{0}'", id));
                }

                return this.View("Delete", comment);
            }
        }

        [HttpPost]
        [MeAuthorize]
        public ActionResult Delete(string lang, int id, string confirmButton)
        {
            using (BlogRepository repository = new BlogRepository())
            {
                Comment comment = repository.LoadComment(id);
                if (comment == null)
                {
                    throw new HttpException(404, string.Format("Couldn't find comment with ID '{0}'", id));
                }

                int postId = comment.PostID;
                repository.DataContext.Comments.DeleteObject(comment);
                repository.DataContext.SaveChanges();
                return this.RedirectToAction("show", "blog", new { id = postId });
            }
        }

        [HttpGet]
        [MeAuthorize]
        public ActionResult Spam(string lang, int id)
        {
            using (BlogRepository repository = new BlogRepository())
            {
                Comment comment = repository.LoadComment(id);
                if (comment == null)
                {
                    throw new HttpException(404, string.Format("Couldn't find comment with ID '{0}'", id));
                }

                comment.IsSpam = true;
                this.spamFilterService.Spam(
                    comment.Body,
                    comment.UserIP,
                    comment.UserAgent,
                    comment.UserName,
                    comment.UserEmail,
                    comment.UserWeb);
                repository.DataContext.SaveChanges();
                return this.RedirectToAction("list");
            }
        }

        [HttpGet]
        [MeAuthorize]
        public ActionResult UnSpam(string lang, int id)
        {
            using (BlogRepository repository = new BlogRepository())
            {
                Comment comment = repository.LoadComment(id);
                if (comment == null)
                {
                    throw new HttpException(404, string.Format("Couldn't find comment with ID '{0}'", id));
                }

                comment.IsSpam = false;
                this.spamFilterService.NotSpam(
                    comment.Body,
                    comment.UserIP,
                    comment.UserAgent,
                    comment.UserName,
                    comment.UserEmail,
                    comment.UserWeb);
                repository.DataContext.SaveChanges();
                repository.SetNotifications(comment);
                return this.RedirectToAction("list");
            }
        }

        [HttpGet]
        [MeAuthorize]
        public ActionResult List(string lang, string action, string controller, int? id)
        {
            using (BlogRepository repository = new BlogRepository())
            {
                int pageIndex = id ?? 1;
                this.ViewData["selectedPage"] = pageIndex;
                int postsCount = repository.GetCommentsCount();
                this.ViewData["pagesCount"] = (postsCount / PagesControl.ItemsPerPage)
                                              + (postsCount % PagesControl.ItemsPerPage == 0 ? 0 : 1);
                return this.View("List", repository.GetComments(pageIndex, PagesControl.ItemsPerPage));
            }
        }
    }
}