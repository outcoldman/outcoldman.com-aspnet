// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Timers
{
    using System;
    using System.Diagnostics;
    using System.Net.Mail;
    using System.Timers;

    using OutcoldSolutions.Web.Blog.Core;
    using OutcoldSolutions.Web.Blog.Core.Util;
    using OutcoldSolutions.Web.Blog.Models.Repositories;
    using OutcoldSolutions.Web.Blog.Resources;

    public class NotificationSender
    {
        private readonly Timer timer;

        public NotificationSender()
        {
            this.timer = new Timer();
            this.timer.Elapsed += this.OnTimed;
            this.timer.AutoReset = false;
            this.timer.Interval = 10000;
            this.timer.Enabled = true;
        }

        private void OnTimed(object state, ElapsedEventArgs elapsedEventArgs)
        {
            try
            {
                using (NotificationRepository repository = new NotificationRepository())
                {
                    foreach (NotificationModel model in repository.GetNotifications())
                    {
                        try
                        {
                            using (SmtpClient client = SmtpConfig.GetClient())
                            {
                                string adminBody = string.Empty;

                                if (string.Compare(model.Notification.Email, ConfigurationUtil.MeEmail, false) == 0
                                    && model.Comment.IsSpam)
                                {
                                    adminBody = "<h2>MARKED AS SPAM!</h2>";
                                }

                                try
                                {
                                    MailMessage message = new MailMessage(
                                        SmtpConfig.GetFrom(), model.Notification.Email)
                                        {
                                            IsBodyHtml = true, 
                                            Subject =
                                                ResourceLoader.GetResource
                                                    (
                                                        model.Language, 
                                                        "NotificationTitle")
                                                + model.Title, 
                                            Body =
                                                string.Format(
                                                    ResourceLoader
                                                .GetResource(
                                                    model.Language, 
                                                    "NotificationBody"), 
                                                    model.Title, 
                                                    model.Comment.Body, 
                                                    string.Format(
                                                        "{2}/{0}/comment/unsubscribe/{1}", 
                                                        model.Language, 
                                                        model
                                                .SubscribtionId, 
                                                        ConfigurationUtil
                                                .SiteUrl), 
                                                    string.Format(
                                                        "{2}/{0}/blog/show/{1}", 
                                                        model.Language, 
                                                        model.Comment
                                                .PostID, 
                                                        ConfigurationUtil
                                                .SiteUrl), 
                                                    string.IsNullOrEmpty(
                                                        model.Comment
                                                        .UserName)
                                                        ? ResourceLoader
                                                              .GetResource
                                                              (
                                                                  model
                                                              .Language, 
                                                                  "Anonymous")
                                                        : model.Comment
                                                              .UserName, 
                                                    adminBody)
                                        };
                                    client.Send(message);
                                }
                                catch (FormatException e)
                                {
                                    Trace.TraceError(e.ToString());
                                }

                                repository.UpdateNotification(model.Notification);
                            }
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError(e.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
            finally
            {
                this.timer.Interval = ConfigurationUtil.GetSettings("NoificationSenderInterval", 120000);
                this.timer.Enabled = true;
            }
        }
    }
}