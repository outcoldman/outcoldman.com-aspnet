// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Metablog
{
    using System.Runtime.Serialization;

    [DataContract]
    public class Source
    {
        [DataMember(Name = "name")]
        public string Name;

        [DataMember(Name = "url")]
        public string Url;
    }
}