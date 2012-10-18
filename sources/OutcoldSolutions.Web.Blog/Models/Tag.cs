namespace OutcoldSolutions.Web.Blog.Models
{
	partial class Tag
	{
		public string LowerName
		{
			get
			{
				return Name.ToLower();
			}
		}
	}
}