using System.Runtime.Serialization;

namespace PersonalWeb.Web.Metablog
{
	[DataContract]
	public class CategoryInfo
	{
		[DataMember(Name = "description")]
		public string Description { get; set; }

		[DataMember(Name = "htmlUrl")]
		public string HtmlUrl { get; set; }

		[DataMember(Name = "rssUrl")]
		public string RssUrl { get; set; }

		[DataMember(Name = "title")]
		public string Title { get; set; }

		[DataMember(Name = "categoryid")]
		public string Categoryid { get; set; }
	}
}