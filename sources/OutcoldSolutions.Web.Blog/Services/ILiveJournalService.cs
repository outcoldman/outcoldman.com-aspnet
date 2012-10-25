// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Services
{
    using System.Collections.Generic;

    using OutcoldSolutions.Web.Blog.Models;

    public interface ILiveJournalService
    {
        IEnumerable<LiveJournalFriendPost> LoadFriendsFeeds(string liveJournalUser);
    }
}