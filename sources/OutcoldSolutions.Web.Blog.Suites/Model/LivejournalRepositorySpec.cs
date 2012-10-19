// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Suites.Model
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using OutcoldSolutions.Web.Blog.Models;
    using OutcoldSolutions.Web.Blog.Models.Repositories;

    [TestFixture]
    public class LivejournalRepositorySpec
    {
        [Test]
        public void Load_Friends_Feeds()
        {
            LivejournalRepository repository = new LivejournalRepository();
            List<LiveJournalFriendPost> liveJournalFriends = repository.LoadFeeds();
            Assert.Greater(liveJournalFriends.Count, 0);
        }
    }
}