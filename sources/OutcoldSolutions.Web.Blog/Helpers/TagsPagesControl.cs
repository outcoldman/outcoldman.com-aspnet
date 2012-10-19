namespace OutcoldSolutions.Web.Blog.Helpers
{
    using System;
    using System.Text;
    using System.Web.Mvc;

    using OutcoldSolutions.Web.Blog.Core.Util;
    using OutcoldSolutions.Web.Blog.Resources;

    public static class TagsPagesControl
	{
		public const int ItemsPerPage = 10;
		public const int PageCenter = 5;

		public static MvcHtmlString CreateTagsPager(this HtmlHelper helper, int selectedPage, int pagesCount)
		{
			if (pagesCount <= 1)
				return MvcHtmlString.Create(string.Empty);

			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<table><tr>");
			stringBuilder.AppendFormat("<td>{0}&nbsp;&nbsp;</td>", helper.GetResource("Page"));

			if (GetNeedStartPoints(selectedPage))
			{
				stringBuilder.AppendFormat("<td style='border:none;'>...&nbsp;&nbsp;</td>");
			}
			for (int i = GetStartPage(selectedPage); i <= GetEndPage(selectedPage, pagesCount); i++)
			{
				if (i == selectedPage)
					stringBuilder.AppendFormat("<td style='border:none;'>{0}&nbsp;&nbsp;</td>", i);
				else
					stringBuilder.AppendFormat("<td style='border:none;'>{0}&nbsp;&nbsp;</td>", CreateLink(helper, i));
			}
			if (GetNeedEndPoints(selectedPage, pagesCount))
			{
				stringBuilder.AppendFormat("<td style='border:none;'>...&nbsp;&nbsp;</td>");
			}
			stringBuilder.Append("</tr></table>");
			return MvcHtmlString.Create(stringBuilder.ToString());
		}

		private static string CreateLink(HtmlHelper helper, int page)
		{
			return string.Format("<a href='{0}'>{1}</a>", NavigationHelper.GetSiteUrl(string.Format("/{0}/blog/tag/{1}/{2}", helper.GetLanguage(), helper.ViewContext.RequestContext.RouteData.Values["tagid"].To(string.Empty), page)), page);
		}

		private static int GetStartPage(int selectedPage)
		{
			if (selectedPage <= PageCenter)
				return 1;
			return selectedPage - PageCenter;
		}

		private static bool GetNeedStartPoints(int selectedPage)
		{
			return GetStartPage(selectedPage) != 1;
		}

		public static int GetEndPage(int selectedPage, int pagesCount)
		{
			if (pagesCount <= (selectedPage + PageCenter))
				return pagesCount;
			return Math.Min(Math.Max(selectedPage, PageCenter) + PageCenter, pagesCount);
		}

		public static bool GetNeedEndPoints(int selectedPage, int pagesCount)
		{
			return GetEndPage(selectedPage, pagesCount) != pagesCount;
		}
	}
}
