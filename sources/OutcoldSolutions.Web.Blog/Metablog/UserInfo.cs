using System.Runtime.Serialization;

namespace PersonalWeb.Web.Metablog
{
	[DataContract]
	public class UserInfo
	{
		[DataMember(Name = "userid")]
		public string Userid;

		[DataMember(Name = "firstname")]
		public string Firstname;

		[DataMember(Name = "lastname")]
		public string Lastname;

		[DataMember(Name = "nickname")]
		public string Nickname;

		[DataMember(Name = "email")]
		public string Email;

		[DataMember(Name = "url")]
		public string Url;
	}
}