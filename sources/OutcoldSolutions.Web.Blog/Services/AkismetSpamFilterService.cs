// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Services
{
    using System;
    using System.Diagnostics;

    using OutcoldSolutions.Web.Blog.Core.Akismet;
    using OutcoldSolutions.Web.Blog.Core.Util;

    public class AkismetSpamFilterService : ISpamFilterService
    {
        private readonly AkismetManager akismetManager;

        public AkismetSpamFilterService()
        {
            this.akismetManager = new AkismetManager("02bdefd7f39c", ConfigurationUtil.SiteUrl);
        }

        public bool IsSpam(string comment, string userIP, string userAgent, string userName, string userEmail, string userWeb)
        {
            try
            {
                AkismetItem commentSpam = new AkismetItem(userIP, userAgent)
                    {
                        AuthorName = userName,
                        AuthorEmail = userEmail,
                        AuthorUrl = userWeb,
                        Content = comment
                    };
                return this.akismetManager.IsSpam(commentSpam);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }

            return false;
        }

        public void Spam(string comment, string userIP, string userAgent, string userName = null, string userEmail = null, string userWeb = null)
        {
            try
            {
                AkismetItem commentSpam = new AkismetItem(userIP, userAgent)
                {
                    AuthorName = userName,
                    AuthorEmail = userEmail,
                    AuthorUrl = userWeb,
                    Content = comment
                };
                this.akismetManager.SubmitSpam(commentSpam);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
        }

        public void NotSpam(string comment, string userIP, string userAgent, string userName = null, string userEmail = null, string userWeb = null)
        {
            try
            {
                AkismetItem commentSpam = new AkismetItem(userIP, userAgent)
                {
                    AuthorName = userName,
                    AuthorEmail = userEmail,
                    AuthorUrl = userWeb,
                    Content = comment
                };
                this.akismetManager.SubmitHam(commentSpam);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
        }
    }
}