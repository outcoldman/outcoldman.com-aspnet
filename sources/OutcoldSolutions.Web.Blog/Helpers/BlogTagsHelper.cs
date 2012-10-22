// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Helpers
{
    using System.Collections.Generic;
    using System.Text;
    using System.Web.Mvc;

    using OutcoldSolutions.Web.Blog.Models;

    public static class BlogTagsHelper
    {
        public static MvcHtmlString RenderTags(this HtmlHelper helper, IList<Tag> tags)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            StringBuilder stringBuilder = new StringBuilder();

            foreach (Tag tag in tags)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(",&nbsp;");
                }

                stringBuilder.AppendFormat(
                    "<a href='{0}' rel='tag'>{1}</a>",
                    urlHelper.Action("tag", "blog", new { lang = helper.GetLanguage(), tagId = tag.TagID, id = 1 }),
                    tag.Name);
            }

            return MvcHtmlString.Create(stringBuilder.ToString());
        }
    }
}