// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Models.Repositories
{
    using System;
    using System.Diagnostics;

    using OutcoldSolutions.Web.Blog.Core.Akismet;
    using OutcoldSolutions.Web.Blog.Core.Util;

    public class AkismetRepository
    {
        public bool IsSpam(Comment comment)
        {
            try
            {
                AkismetItem commentSpam = this.GetItem(comment);
                return this.GetManager().IsSpam(commentSpam);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }

            return false;
        }

        public void SubmitSpam(Comment comment)
        {
            try
            {
                AkismetItem commentSpam = this.GetItem(comment);
                this.GetManager().SubmitSpam(commentSpam);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
        }

        public void SubmitUnSpam(Comment comment)
        {
            try
            {
                AkismetItem commentSpam = this.GetItem(comment);
                this.GetManager().SubmitHam(commentSpam);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
        }

        private AkismetManager GetManager()
        {
            return new AkismetManager("02bdefd7f39c", ConfigurationUtil.SiteUrl);
        }

        private AkismetItem GetItem(Comment comment)
        {
            return new AkismetItem(comment.UserIP, comment.UserAgent)
                {
                    AuthorName = comment.UserName, 
                    AuthorEmail = comment.UserEmail, 
                    AuthorUrl = comment.UserWeb, 
                    Content = comment.Body
                };
        }
    }
}