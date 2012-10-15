using System.Web.Mvc;
using PersonalWeb.Web.Controls;

namespace PersonalWeb.Web.Resources
{
	public static class ResourceLoader
	{
		public static string GetResource(this HtmlHelper helper, string resource)
		{
			if (helper.IsRussian())
				return ResourceRu.ResourceManager.GetString(resource);
			return ResourceEn.ResourceManager.GetString(resource);
		}

		public static string GetResource(string lang, string resource)
		{
			if (lang.ToLower() == "ru")
				return ResourceRu.ResourceManager.GetString(resource);
			return ResourceEn.ResourceManager.GetString(resource);
		}

	}
}