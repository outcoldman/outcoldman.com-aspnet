// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Metablog
{
    using System.Runtime.Serialization;

    [DataContract]
    public class UserInfo
    {
        [DataMember(Name = "email")]
        public string Email;

        [DataMember(Name = "firstname")]
        public string Firstname;

        [DataMember(Name = "lastname")]
        public string Lastname;

        [DataMember(Name = "nickname")]
        public string Nickname;

        [DataMember(Name = "url")]
        public string Url;

        [DataMember(Name = "userid")]
        public string Userid;
    }
}