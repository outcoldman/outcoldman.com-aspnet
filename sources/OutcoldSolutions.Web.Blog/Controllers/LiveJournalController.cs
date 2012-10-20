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
    using OutcoldSolutions.Web.Blog.Services;

    public class LiveJournalController : Controller
    {
        private readonly ILiveJournalService liveJournalService;

        public LiveJournalController(ILiveJournalService liveJournalService)
        {
            this.liveJournalService = liveJournalService;
        }

        [OutputCache(Duration = 1800, VaryByParam = "none")]
        public ActionResult Friends()
        {
            var liveJournalPosts =
                this.liveJournalService.LoadFriendsFeeds("outcoldman")
                    .Select(i => new SyndicationItem(i.Title, i.Body, new Uri(i.Link)) { LastUpdatedTime = i.Date });

            var feed = new SyndicationFeed(
                "outcoldman LJ Friends",
                "outcoldman LiveJournal Friends",
                new Uri("http://outcoldman.livejournal.com/friends"),
                liveJournalPosts);

            return new FeedResult(new Rss20FeedFormatter(feed));
        }
    }
}