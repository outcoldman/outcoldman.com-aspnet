using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PersonalWeb.Model;
using PersonalWeb.Model.Repositories;

namespace PersonalWeb.Tests.Model
{
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
