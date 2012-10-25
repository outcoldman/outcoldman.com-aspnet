// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
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
            return new SmtpClient()
                {
                    Credentials =
                        new NetworkCredential(
                        ConfigurationUtil.GetSettings("SmtpUserName", "Author"),
                        ConfigurationUtil.GetSettings("SmtpPassword", "Password"))
                };
        }

        public static string GetFrom()
        {
            return string.Format("no-reply@{0}", new Uri(ConfigurationUtil.SiteUrl).DnsSafeHost);
        }
    }
}