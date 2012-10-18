using System.Runtime.Serialization;

namespace PersonalWeb.Web.Metablog
{
	[DataContract]
	public class Enclosure
	{
		[DataMember(Name = "length")]
		public int Length { get; set; }

		[DataMember(Name = "type")]
		public string Type { get; set; }

		[DataMember(Name = "url")]
		public string Url { get; set; }
	}
}