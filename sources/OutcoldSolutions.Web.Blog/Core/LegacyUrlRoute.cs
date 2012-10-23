// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Core
{
    using System;
    using System.Web;
    using System.Web.Routing;

    using OutcoldSolutions.Web.Blog.Core.Util;

    public class LegacyUrlRoute : RouteBase
    {
        private readonly Uri currentUri = new Uri(ConfigurationUtil.SiteUrl);

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var request = httpContext.Request;
            var response = httpContext.Response;

            if (request.Url != null)
            {
                var uriBuilder = new UriBuilder(request.Url)
                    {
                        Path = string.Empty,
                        Query = string.Empty
                    };

                if (!uriBuilder.Uri.Equals(this.currentUri))
                {
                    var rightUri = new UriBuilder(this.currentUri) { Path = request.Url.PathAndQuery };

                    response.StatusCode = 301; /* HTTP_STATUS_MOVED */
                    response.RedirectLocation = rightUri.Uri.ToString();
                    response.End();
                }
            }

            return null;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return null;
        }
    }

}