// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------
namespace OutcoldSolutions.Web.Blog.Core
{
    using System;
    using System.ServiceModel.Syndication;
    using System.Web;
    using System.Web.Mvc;
    using System.Xml;

    public class FeedResult : ActionResult
    {
        private readonly SyndicationFeedFormatter feed;

        public FeedResult(SyndicationFeedFormatter feed)
        {
            if (feed == null)
            {
                throw new ArgumentNullException("feed");
            }

            this.feed = feed;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/rss+xml";

            using (var xmlWriter = new XmlTextWriter(response.Output))
            {
                xmlWriter.Formatting = Formatting.Indented;
                this.feed.WriteTo(xmlWriter);
            }
        }
    }
}