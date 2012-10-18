using System.Text;

namespace OutcoldSolutions.Web.Blog.Models
{
	public partial class BlogPost
	{
		public string TagsLine
		{
			get
			{
				StringBuilder tags = new StringBuilder();

				foreach (Tag tag in Tags)
				{
					if (tags.Length > 0)
						tags.Append(", ");
					tags.Append(tag.Name);
				}

				return tags.ToString();
			}
		}

		public int CommentsCount
		{
			get;
			set;
		}
	}
}
