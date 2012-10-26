// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Services
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;

    using HtmlAgilityPack;

    using OutcoldSolutions.Web.Blog.Core.Util;
    using OutcoldSolutions.Web.Blog.Models;

    public class LiveJournalService : ILiveJournalService
    {
        public IEnumerable<LiveJournalFriendPost> LoadFriendsFeeds(string liveJournalUser)
        {
            HtmlWeb hw = new HtmlWeb();
            var friendsUrl = string.Format(CultureInfo.InvariantCulture, "http://{0}.livejournal.com/friends", HttpUtility.UrlEncode(liveJournalUser));
            HtmlDocument doc = hw.Load(friendsUrl);

            HtmlNodeCollection entries = doc.DocumentNode.SelectNodes("//div[@class='subcontent']");

            foreach (HtmlNode entry in entries)
            {
                LiveJournalFriendPost item = new LiveJournalFriendPost();

                HtmlNode href = entry.SelectSingleNode(".//a[@href and @class='subj-link']");
                HtmlNode friend = entry.SelectSingleNode(".//font[@color='#000000']");

                if (friend != null)
                {
                    item.Title = string.Format(CultureInfo.InvariantCulture, "{0}: ", friend.InnerText);
                }

                // get what's interesting for RSS 
                if (href != null)
                {
                    item.Link = href.Attributes["href"].Value;
                    item.Title += href.InnerText;
                }

                if (string.IsNullOrEmpty(item.Link))
                {
                    item.Link = friendsUrl;
                }

                HtmlNode date = entry.SelectSingleNode(".//div[@class='date']");

                if (date != null)
                {
                    var s = date.InnerText.Trim(new[] { ' ', '\n', '\r', '\t' });
                    item.Date = Parser.ToDateTime(s, "dd MMMM yyyy @ hh:mm tt");
                }

                HtmlNode descNode = entry.SelectSingleNode(".//div[@class='entry_text']");
                if (descNode != null)
                {
                    item.Body = descNode.InnerHtml;
                }

                yield return item;
            }
        }
    }
}