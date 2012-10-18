using System.Runtime.Serialization;

namespace PersonalWeb.Web.Metablog
{
	[DataContract]
	public class MediaObjectInfo
	{
		[DataMember(Name = "url")]
		public string Url;
	}
}