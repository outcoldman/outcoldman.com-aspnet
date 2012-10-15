using System;
using System.Net.Mail;
using System.Timers;
using PersonalWeb.Core.Util;
using PersonalWeb.Model.Repositories;
using PersonalWeb.Web.Resources;

namespace PersonalWeb.Web.Timers
{
	public class NotificationSender
	{
		private readonly Timer _timer;

		public NotificationSender()
		{
			_timer = new Timer();
			_timer.Elapsed += OnTimed;
			_timer.AutoReset = false;
			_timer.Interval = 10000;
			_timer.Enabled = true;
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

								if (string.Compare(model.Notification.Email, ConfigurationUtil.MeEmail, false) == 0 && model.Comment.IsSpam)
								{
									adminBody = "<h2>MARKED AS SPAM!</h2>";
								}

								try
								{
									MailMessage message = new MailMessage(SmtpConfig.GetFrom(), model.Notification.Email)
									{
										IsBodyHtml = true,
										Subject =
											ResourceLoader.GetResource(model.Language, "NotificationTitle") + model.Title,
										Body =
											string.Format(ResourceLoader.GetResource(model.Language, "NotificationBody"),
														  model.Title,
														  model.Comment.Body,
														  string.Format("{2}/{0}/comment/unsubscribe/{1}",
																		model.Language, model.SubscribtionId,
																		ConfigurationUtil.SiteUrl),
														  string.Format("{2}/{0}/blog/show/{1}",
																		model.Language, model.Comment.PostID,
																		ConfigurationUtil.SiteUrl),
														  string.IsNullOrEmpty(model.Comment.UserName)
															? ResourceLoader.GetResource(model.Language, "Anonymous")
															: model.Comment.UserName, adminBody)
									};
									client.Send(message);
								}
								catch (FormatException e)
								{
									Log4NetHelper.Log.Error(e);
								}
								
								repository.UpdateNotification(model.Notification);
							}
						}
						catch (Exception e)
						{
							Log4NetHelper.Log.Error(e);
						}
					}
				}
			}
			catch (Exception e)
			{
				Log4NetHelper.Log.Error(e);
			}
			finally
			{
				_timer.Interval = ConfigurationUtil.GetSettings("NoificationSenderInterval", 120000);
				_timer.Enabled = true;
			}
		}
	}
}