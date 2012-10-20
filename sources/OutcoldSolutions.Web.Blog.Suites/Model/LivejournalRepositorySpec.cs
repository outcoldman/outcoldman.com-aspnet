// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Suites.Model
{
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    using OutcoldSolutions.Web.Blog.Models;
    using OutcoldSolutions.Web.Blog.Services;

    [TestFixture]
    public class LivejournalRepositorySpec
    {
        [Test]
        public void Load_Friends_Feeds()
        {
            LiveJournalService repository = new LiveJournalService();
            IEnumerable<LiveJournalFriendPost> liveJournalFriends = repository.LoadFriendsFeeds("outcoldman");
            Assert.Greater(liveJournalFriends.Count(), 0);
        }
    }
}