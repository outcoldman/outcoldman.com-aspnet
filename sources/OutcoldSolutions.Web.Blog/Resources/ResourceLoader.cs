// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Resources
{
    using System.Web.Mvc;

    using OutcoldSolutions.Web.Blog.Helpers;

    public static class ResourceLoader
    {
        public static string GetResource(this HtmlHelper helper, string resource)
        {
            if (helper.IsRussian())
            {
                return ResourceRu.ResourceManager.GetString(resource);
            }

            return ResourceEn.ResourceManager.GetString(resource);
        }

        public static string GetResource(string lang, string resource)
        {
            if (lang.ToLower() == "ru")
            {
                return ResourceRu.ResourceManager.GetString(resource);
            }

            return ResourceEn.ResourceManager.GetString(resource);
        }
    }
}