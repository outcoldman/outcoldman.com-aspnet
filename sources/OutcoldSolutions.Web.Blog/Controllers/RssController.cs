// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Controllers
{
    using System;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using System.Web.Mvc;

    using OutcoldSolutions.Web.Blog.Core;
    using OutcoldSolutions.Web.Blog.Models.Repositories;

    public class RssController : Controller
    {
        [CustomOutputCache]
        [ValidateLang]
        public ActionResult Index(string lang)
        {
            using (BlogRepository blogRepository = new BlogRepository(BlogPostLoadFlag.LoadTags | BlogPostLoadFlag.LoadBody))
            {
                var articles = blogRepository.GetRss(lang, false);

                var requestUrl = this.HttpContext.Request.Url;

                var blogArticles =
                    articles
                        .Select(i =>
                            {
                                var url = new Uri(this.Url.Action("show", "blog", new { lang = i.Language, id = i.PostID }, requestUrl.Scheme));
                                var syndicationItem = new SyndicationItem(i.Title, i.HtmlText, url) { LastUpdatedTime = i.Date };

                                foreach (var tag in i.Tags)
                                {
                                    syndicationItem.Categories.Add(new SyndicationCategory(tag.Name));
                                }

                                return syndicationItem;
                            });

                var uriBuilder = new UriBuilder(requestUrl.Scheme, requestUrl.Host, requestUrl.Port, lang);

                var syndicationFeed = new SyndicationFeed(
                    "outcoldman",
                    "Denis Gladkikh Personal Web",
                    uriBuilder.Uri,
                    blogArticles)
                    {
                        LastUpdatedTime = DateTime.Now
                    };

                return new FeedResult(new Rss20FeedFormatter(syndicationFeed));
            }
        }
    }
}