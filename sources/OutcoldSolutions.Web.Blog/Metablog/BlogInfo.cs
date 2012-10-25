// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Metablog
{
    using System.Runtime.Serialization;

    [DataContract(Namespace = "http://www.blogger.com/developers/api/1_docs/")]
    public class BlogInfo
    {
        [DataMember(Name = "blogid")]
        public string BlogId { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "blogName")]
        public string BlogName { get; set; }
    }
}