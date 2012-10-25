// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Controllers
{
    using System;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;

    using OutcoldSolutions.Web.Blog.Core;
    using OutcoldSolutions.Web.Blog.Helpers;
    using OutcoldSolutions.Web.Blog.Models;
    using OutcoldSolutions.Web.Blog.Models.Repositories;

    public class BlogController : Controller
    {
        [HttpGet]
        [CustomOutputCache]
        [ValidateLang]
        public ActionResult Index(string lang, int? id)
        {
            using (var repository = new BlogRepository(BlogPostLoadFlag.LoadAbstraction | BlogPostLoadFlag.LoadTags))
            {
                int pageIndex = id ?? 1;
                this.ViewData["selectedPage"] = pageIndex;

                int postsCount = repository.GetPostsCount(lang);
                this.ViewData["pagesCount"] = (postsCount / PagesControl.ItemsPerPage)
                                              + (postsCount % PagesControl.ItemsPerPage == 0 ? 0 : 1);

                this.ViewData["toptags"] = repository.GetTopTags(lang);

                return this.View("Index", repository.GetPosts(pageIndex, PagesControl.ItemsPerPage, lang));
            }
        }

        [HttpGet]
        [CustomOutputCache]
        [ValidateLang]
        public ActionResult Tag(string lang, int tagid, int? id)
        {
            using (var repository = new BlogRepository(BlogPostLoadFlag.LoadAbstraction | BlogPostLoadFlag.LoadTags))
            {
                int pageIndex = id ?? 1;
                this.ViewData["selectedPage"] = pageIndex;
                Tag tag = repository.GetTag(tagid);

                if (tag == null)
                {
                    throw new HttpException(
                        (int)HttpStatusCode.NotFound, string.Format("Couldn't find tag with ID {0}", id));
                }

                this.ViewData["tag"] = tag;
                int postsCount = repository.GetPostsByTagCount(lang, tagid);
                this.ViewData["pagesCount"] = (postsCount / PagesControl.ItemsPerPage)
                                              + (postsCount % PagesControl.ItemsPerPage == 0 ? 0 : 1);

                this.ViewData["toptags"] = repository.GetTopTags(lang);

                return this.View("Index", repository.GetPostsByTag(pageIndex, PagesControl.ItemsPerPage, lang, tagid));
            }
        }

        [HttpGet]
        [CustomOutputCache]
        [ValidateLang]
        public ActionResult Show(string lang, int id)
        {
            using (var blogRepository = new BlogRepository(BlogPostLoadFlag.LoadBody | BlogPostLoadFlag.LoadTags))
            {
                BlogPost blogPost = blogRepository.LoadPost(id);
                this.CheckBlogPostExist(id, lang, blogPost);
                this.ViewData["comments"] = blogRepository.GetComments(id);

                // For master page
                this.ViewData["keywords"] = blogPost.TagsLine;
                this.ViewData["description"] = this.Server.HtmlEncode(blogPost.Title);

                // ------
                this.ViewData["simpleposts"] = blogRepository.GetLikePosts(id);
                return this.View("ItemView", blogPost);
            }
        }

        [HttpGet]
        [MeAuthorize]
        public ActionResult ItemEdit(string lang, int id)
        {
            using (var blogRepository = new BlogRepository(BlogPostLoadFlag.FullLoad))
            {
                BlogPost blogPost = blogRepository.LoadPost(id);
                this.CheckBlogPostExist(id, lang, blogPost);
                return this.View("ItemEdit", blogPost);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [MeAuthorize]
        public ActionResult ItemEdit(string lang, int id, FormCollection formValues)
        {
            using (var blogRepository = new BlogRepository(BlogPostLoadFlag.FullLoad))
            {
                BlogPost blogPost = blogRepository.LoadPost(id);
                this.CheckBlogPostExist(id, lang, blogPost);

                this.ValidateBlogPost(blogPost, formValues);

                if (this.ModelState.IsValid)
                {
                    this.UpdateModel(blogPost);
                    blogRepository.Save(blogPost, formValues["TagsLine"]);
                    return this.RedirectToAction("show", "blog", new { id, lang = blogPost.Language });
                }

                return this.View(blogPost);
            }
        }

        [HttpGet]
        [MeAuthorize]
        public ActionResult Create(string lang)
        {
            var blogPost = new BlogPost { Date = DateTime.Now.ToUniversalTime() };
            return this.View("ItemEdit", blogPost);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MeAuthorize]
        public ActionResult Create(string lang, FormCollection formValues)
        {
            using (var blogRepository = new BlogRepository())
            {
                var blogPost = new BlogPost();

                this.ValidateBlogPost(blogPost, formValues);

                if (this.ModelState.IsValid)
                {
                    this.UpdateModel(blogPost);
                    blogRepository.Save(blogPost, formValues["TagsLine"]);
                    return this.RedirectToAction("show", "blog", new { id = blogPost.PostID, lang = blogPost.Language });
                }

                return this.View("ItemEdit", blogPost);
            }
        }

        [HttpGet]
        [MeAuthorize]
        public ActionResult List(string lang, string action, string controller, int? id)
        {
            using (var repository = new BlogRepository())
            {
                int pageIndex = id ?? 1;
                this.ViewData["selectedPage"] = pageIndex;
                int postsCount = repository.GetPostsCount();
                this.ViewData["pagesCount"] = (postsCount / PagesControl.ItemsPerPage)
                                              + (postsCount % PagesControl.ItemsPerPage == 0 ? 0 : 1);
                return this.View("List", repository.GetPosts(pageIndex, PagesControl.ItemsPerPage));
            }
        }

        [HttpGet]
        [MeAuthorize]
        public ActionResult Delete(string lang, int id)
        {
            using (BlogRepository repository = new BlogRepository())
            {
                BlogPost blogPost = repository.LoadPost(id);
                this.CheckBlogPostExist(id, lang, blogPost);
                return this.View("Delete", blogPost);
            }
        }

        [HttpPost]
        [MeAuthorize]
        public ActionResult Delete(string lang, int id, string confirmButton)
        {
            using (BlogRepository repository = new BlogRepository())
            {
                BlogPost blogPost = repository.LoadPost(id);
                this.CheckBlogPostExist(id, lang, blogPost);
                repository.DataContext.BlogPosts.DeleteObject(blogPost);
                repository.DataContext.SaveChanges();
                return this.RedirectToAction("index");
            }
        }

        private void ValidateBlogPost(BlogPost blogPost, FormCollection formValues)
        {
            blogPost.Title = formValues["Title"];
            if (string.IsNullOrEmpty(blogPost.Title))
            {
                this.AddCannotEmptyError("Title", "Title");
            }

            string strDate = formValues["Date"];
            DateTime date;
            if (DateTime.TryParse(strDate, out date))
            {
                blogPost.Date = date;
            }
            else
            {
                this.ModelState.AddModelError("Date", @"Check that date in correct format.");
            }

            blogPost.HtmlAbstraction = formValues["HtmlAbstraction"];
            if (string.IsNullOrEmpty(blogPost.HtmlAbstraction))
            {
                this.AddCannotEmptyError("HtmlAbstraction", "Abstraction");
            }

            blogPost.HtmlText = formValues["HtmlText"];
            if (string.IsNullOrEmpty(blogPost.HtmlText))
            {
                this.AddCannotEmptyError("HtmlText", "Body cannot be empty");
            }
        }

        private void AddCannotEmptyError(string field, string name)
        {
            this.ModelState.AddModelError(field, string.Format("{0} cannot be empty", name));
        }

        private void CheckBlogPostExist(int id, string lang, BlogPost blogPost)
        {
            if (blogPost == null || blogPost.PostID != id || string.Compare(blogPost.Language, lang, true) != 0
                || ((!blogPost.IsPublic || blogPost.Date > DateTime.UtcNow) && !this.HttpContext.IsMe()))
            {
                throw new HttpException(
                    (int)HttpStatusCode.NotFound, string.Format("Couldn't find blog post with ID {0}", id));
            }
        }
    }
}