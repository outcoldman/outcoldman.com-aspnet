namespace OutcoldSolutions.Web.Blog.Core
{
    using System.Net;
    using System.Net.Mail;

    public static class SmtpConfig
	{
		public static SmtpClient GetClient()
		{
			return new SmtpClient("smtp.outcoldman.ru", 2525)
			{
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential("bot@outcoldman.ru", "botQw12er34")
			};
		}

		public static string GetFrom()
		{
			return "bot@outcoldman.ru";
		}
	}
}
