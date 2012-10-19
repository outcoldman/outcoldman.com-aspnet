// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Metablog
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class Post
    {
        [DataMember(Name = "dateCreated")]
        public DateTime DateCreated { get; set; }

        [DataMember(Name = "date_created_gmt")]
        public DateTime DateCreatedGmt { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "categories")]
        public string[] Categories { get; set; }

        [DataMember(Name = "permalink")]
        public string Permalink { get; set; }

        [DataMember(Name = "postid")]
        public object Postid { get; set; }

        [DataMember(Name = "userid")]
        public string Userid { get; set; }

        [DataMember(Name = "wp_slug")]
        public string WpSlug { get; set; }

        [DataMember(Name = "publish")]
        public bool Publish { get; set; }
    }
}