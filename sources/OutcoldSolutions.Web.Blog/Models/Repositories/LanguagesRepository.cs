namespace OutcoldSolutions.Web.Blog.Models.Repositories
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    public static class LanguagesRepository
	{
		public static IList<SelectListItem> GetLanguages()
		{
			return new List<SelectListItem>
			       	{
			       		new SelectListItem {Text = "Russian", Value = "ru"},
			       		new SelectListItem {Text = "English", Value = "en"}
			       	};
		}
	}
}