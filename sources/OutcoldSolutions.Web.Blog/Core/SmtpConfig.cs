// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Core
{
    using System;
    using System.Net;
    using System.Net.Mail;

    using OutcoldSolutions.Web.Blog.Core.Util;

    public static class SmtpConfig
    {
        public static SmtpClient GetClient()
        {
            return new SmtpClient(ConfigurationUtil.GetSettings("SmtpServer", "example.com"), ConfigurationUtil.GetSettings("SmtpPort", 25))
                {
                    UseDefaultCredentials = false, 
                    Credentials = new NetworkCredential("bot@outcoldman.ru", "botQw12er34")
                };
        }

        public static string GetFrom()
        {
            return string.Format("no-reply@{0}", new Uri(ConfigurationUtil.SiteUrl).DnsSafeHost);
        }
    }
}