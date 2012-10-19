// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Models.Repositories
{
    using System.Collections.Generic;

    using HtmlAgilityPack;

    using OutcoldSolutions.Web.Blog.Core.Util;

    public class LivejournalRepository : BaseRepository
    {
        public List<LiveJournalFriendPost> LoadFeeds()
        {
            List<LiveJournalFriendPost> list = new List<LiveJournalFriendPost>();

            HtmlWeb hw = new HtmlWeb();
            HtmlDocument doc = hw.Load("http://outcoldman.livejournal.com/friends");

            HtmlNodeCollection entries = doc.DocumentNode.SelectNodes("//div[@class='subcontent']");

            foreach (HtmlNode entry in entries)
            {
                LiveJournalFriendPost item = new LiveJournalFriendPost();

                HtmlNode href = entry.SelectSingleNode("//a[@href and @class='subj-link']");
                HtmlNode friend = entry.SelectSingleNode("//font[@color='#000000']");

                // get what's interesting for RSS 
                if (href != null)
                {
                    item.Link = href.Attributes["href"].Value;
                    if (friend != null)
                    {
                        item.Title = string.Format("{0}: ", friend.InnerText);
                    }

                    item.Title += href.InnerText;
                }

                HtmlNode date = entry.SelectSingleNode("//div[@class='date']");

                if (date != null)
                {
                    var s = date.InnerText.Trim(new[] { ' ', '\n', '\r', '\t' });
                    item.Date = Parser.ToDateTime(s, "dd MMMM yyyy @ hh:mm tt");
                }

                HtmlNode descNode = entry.SelectSingleNode("//div[@class='entry_text']");
                if (descNode != null)
                {
                    item.Body = descNode.InnerHtml;
                }

                list.Add(item);
            }

            return list;
        }
    }
}