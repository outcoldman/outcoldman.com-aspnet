// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Metablog
{
    using System.Runtime.Serialization;

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