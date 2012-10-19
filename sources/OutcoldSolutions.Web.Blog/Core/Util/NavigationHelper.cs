namespace OutcoldSolutions.Web.Blog.Core.Util
{
    using System.Web;
    using System;

    public class NavigationHelper
    {
        public static string GetUrlWithHost(string url)
        {
            Uri requestUri = HttpContext.Current.Request.Url;
            return string.Format("{0}://{1}{2}", requestUri.Scheme,
                                 requestUri.Authority, PlaceSeparator(url));
        }

        public static string GetSiteUrl(string url)
        {
            if (HttpContext.Current.Request.ApplicationPath.Length > 1)
            {
                url = PlaceSeparator(url);
                url = string.Format("{0}{1}", HttpContext.Current.Request.ApplicationPath, url);
            }
            return GetUrlWithHost(url.Replace("\\", "/"));
        }

        private static string PlaceSeparator(string url)
        {
            if (!string.IsNullOrEmpty(url) && url.Length > 0 && url[0] != '\\' && url[0] != '/')
                url = '/' + url;
            return url;
        }
    }
}