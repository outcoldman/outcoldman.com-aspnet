// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Models
{
    using System;

    public class LiveJournalFriendPost
    {
        public LiveJournalFriendPost()
        {
            this.Title = string.Empty;
            this.Body = string.Empty;
            this.Link = string.Empty;
        }

        public DateTime Date { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string Link { get; set; }
    }
}