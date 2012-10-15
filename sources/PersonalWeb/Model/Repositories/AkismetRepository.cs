using System;
using PersonalWeb.Core.Akismet;
using PersonalWeb.Core.Util;

namespace PersonalWeb.Model.Repositories
{
	public class AkismetRepository
	{
		public bool IsSpam(Comment comment)
		{
			try
			{
				AkismetItem commentSpam = GetItem(comment);
				return GetManager().IsSpam(commentSpam);
			}
			catch (Exception e)
			{
				Log4NetHelper.Log.Error(e);
			}
			return false;
		}

		public void SubmitSpam(Comment comment)
		{
			try
			{
				AkismetItem commentSpam = GetItem(comment);
				GetManager().SubmitSpam(commentSpam);
			}
			catch (Exception e)
			{
				Log4NetHelper.Log.Error(e);
			}
		}

		public void SubmitUnSpam(Comment comment)
		{
			try
			{
				AkismetItem commentSpam = GetItem(comment);
				GetManager().SubmitHam(commentSpam);
			}
			catch (Exception e)
			{
				Log4NetHelper.Log.Error(e);
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