using System.Runtime.Serialization;

namespace PersonalWeb.Web.Metablog
{
	[DataContract]
	public class Source
	{
		[DataMember(Name = "name")]
		public string Name;

		[DataMember(Name = "url")]
		public string Url;
	}
}