// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Models
{
    using System.Text;

    public partial class BlogPost
    {
        public string TagsLine
        {
            get
            {
                StringBuilder tags = new StringBuilder();

                foreach (Tag tag in this.Tags)
                {
                    if (tags.Length > 0)
                    {
                        tags.Append(", ");
                    }

                    tags.Append(tag.Name);
                }

                return tags.ToString();
            }
        }

        public int CommentsCount { get; set; }
    }
}