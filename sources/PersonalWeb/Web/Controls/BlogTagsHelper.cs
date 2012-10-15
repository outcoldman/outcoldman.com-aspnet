using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using PersonalWeb.Core.Util;
using PersonalWeb.Model;

namespace PersonalWeb.Web.Controls
{
	public static class BlogTagsHelper
	{
		public static MvcHtmlString RenderTags(this HtmlHelper helper, IList<Tag> tags)
		{
			StringBuilder stringBuilder = new StringBuilder();

			foreach (Tag tag in tags)
			{
				if (stringBuilder.Length > 0)
					stringBuilder.Append(",&nbsp;");
				//stringBuilder.Append(
				//helper.ActionLink(tag.Tag.Name, "tag", "blog", new RouteValueDictionary() {{"tagid", tag.TagID}, {"page", 1}}, null).ToString());
				stringBuilder.AppendFormat("<a href='{0}' rel='tag'>{1}</a>",
				                           NavigationHelper.GetSiteUrl(string.Format("/{0}/blog/tag/{1}", helper.GetLanguage(),
				                                                                     tag.TagID)), tag.Name);
			}
			return MvcHtmlString.Create(stringBuilder.ToString());
		}
	}
}