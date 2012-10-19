// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    using OutcoldSolutions.Web.Blog.Core;
    using OutcoldSolutions.Web.Blog.Models;
    using OutcoldSolutions.Web.Blog.Models.Repositories;

    public class FeedModel
    {
        public string Language { get; set; }

        public bool IsExternal { get; set; }

        public List<BlogPost> Items { get; set; }
    }

    public class RssController : Controller
    {
        [CustomOutputCache]
        [ValidateLang]
        public ActionResult Index(string lang, string id)
        {
            FeedModel feed = new FeedModel { Language = lang, IsExternal = id.ToLower() == "ext" };

            BlogPostLoadFlag flag = BlogPostLoadFlag.LoadTags;
            if (feed.IsExternal)
            {
                flag |= BlogPostLoadFlag.LoadAbstraction;
            }
            else
            {
                flag |= BlogPostLoadFlag.LoadBody;
            }

            using (BlogRepository blogRepository = new BlogRepository(flag))
            {
                feed.Items = blogRepository.GetRss(lang, feed.IsExternal);
            }

            return this.View("rss", feed);
        }
    }
}