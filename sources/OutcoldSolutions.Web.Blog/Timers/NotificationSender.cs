﻿// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Timers
{
    using System;
    using System.Diagnostics;
    using System.Net.Mail;

    using OutcoldSolutions.Web.Blog.Core;
    using OutcoldSolutions.Web.Blog.Core.Util;
    using OutcoldSolutions.Web.Blog.Models;
    using OutcoldSolutions.Web.Blog.Models.Repositories;
    using OutcoldSolutions.Web.Blog.Resources;

    public class NotificationSender
    {
        public void SendNotifications(Comment comment)
        {
            try
            {
                using (NotificationRepository repository = new NotificationRepository())
                {
                    foreach (NotificationModel model in repository.GetNotifications(comment))
                    {
                        try
                        {
                            using (SmtpClient client = SmtpConfig.GetClient())
                            {
                                string adminBody = string.Empty;

                                if (string.Equals(model.Notification.Email, ConfigurationUtil.AuthorEmail, StringComparison.OrdinalIgnoreCase) && model.Comment.IsSpam)
                                {
                                    adminBody = "<h2>MARKED AS SPAM!</h2>";
                                }

                                try
                                {
                                    MailMessage message = new MailMessage(
                                        SmtpConfig.GetFrom(), model.Notification.Email)
                                        {
                                            IsBodyHtml = true, 
                                            Subject = ResourceLoader.GetResource(model.Language, "NotificationTitle") + model.Title, 
                                            Body = string.Format(
                                                    ResourceLoader.GetResource(model.Language, "NotificationBody"), 
                                                    model.Title, 
                                                    model.Comment.Body, 
                                                    string.Format("{2}/{0}/comment/unsubscribe/{1}", model.Language, model.SubscribtionId, ConfigurationUtil.SiteUrl), 
                                                    string.Format("{2}/{0}/blog/show/{1}", model.Language, model.Comment.PostID, ConfigurationUtil.SiteUrl), 
                                                    string.IsNullOrEmpty(model.Comment.UserName) ? ResourceLoader.GetResource(model.Language, "Anonymous") : model.Comment.UserName, 
                                                    adminBody),
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
        }
    }
}