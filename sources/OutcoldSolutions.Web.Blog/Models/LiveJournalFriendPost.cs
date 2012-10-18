using System;

namespace PersonalWeb.Model
{
	public class LiveJournalFriendPost
	{
		public LiveJournalFriendPost()
		{
			Title = string.Empty;
			Body = string.Empty;
			Link = string.Empty;
		}

		public DateTime Date { get; set; }
		public string Title { get; set; }
		public string Body { get; set; }
		public string Link { get; set; }
	}
}