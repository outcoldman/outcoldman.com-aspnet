// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UserProfile")]
    internal class Article
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("PostID")]
        public int PostId { get; set; }

        public string Title { get; set; }

        public string Language { get; set; }

        public string HtmlText { get; set; }

        public DateTime Date { get; set; }

        public bool IsPublic { get; set; }

        public byte[] DateChanged { get; set; }

        public string HtmlAbstraction { get; set; }

        public bool IsJustAbstraction { get; set; }

        public bool IsForExternal { get; set; }
    }
}