// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Helpers
{
    using System;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    
    public static class PagesControl
    {
        public const int ItemsPerPage = 10;

        public const int PageCenter = 5;

        public static MvcHtmlString CreatePager(this HtmlHelper helper, int selectedPage, int pagesCount)
        {
            if (pagesCount <= 1)
            {
                return MvcHtmlString.Create(string.Empty);
            }
            
            StringBuilder stringBuilder = new StringBuilder();

            if (GetNeedStartPoints(selectedPage))
            {
                stringBuilder.AppendFormat("<span>...&nbsp;&nbsp;</span>");
            }

            for (int i = GetStartPage(selectedPage); i <= GetEndPage(selectedPage, pagesCount); i++)
            {
                if (i == selectedPage)
                {
                    stringBuilder.AppendFormat("<span>{0}&nbsp;&nbsp;</span>", i);
                }
                else
                {
                    stringBuilder.AppendFormat("<span>{0}&nbsp;&nbsp;</span>", CreateLink(helper, i));
                }
            }

            if (GetNeedEndPoints(selectedPage, pagesCount))
            {
                stringBuilder.AppendFormat("<span>...&nbsp;&nbsp;</span>");
            }

            return MvcHtmlString.Create(stringBuilder.ToString());
        }

        private static string CreateLink(HtmlHelper helper, int page)
        {
            var tagId = helper.ViewContext.RequestContext.RouteData.Values["tagid"];
            string sPage = page.ToString();
            if (tagId == null)
            {

                return
                    helper.ActionLink(sPage, helper.GetAction(), helper.GetController(), new { id = page }, null).ToString();
            }
            else
            {
                return helper.ActionLink(sPage, "tag", "blog", new { lang = helper.GetLanguage(), tagId = helper.ViewContext.RequestContext.RouteData.Values["tagid"], id = page }, null).ToString();
            }
        }

        private static int GetStartPage(int selectedPage)
        {
            if (selectedPage <= PageCenter)
            {
                return 1;
            }

            return selectedPage - PageCenter;
        }

        private static bool GetNeedStartPoints(int selectedPage)
        {
            return GetStartPage(selectedPage) != 1;
        }

        public static int GetEndPage(int selectedPage, int pagesCount)
        {
            if (pagesCount <= (selectedPage + PageCenter))
            {
                return pagesCount;
            }

            return Math.Min(Math.Max(selectedPage, PageCenter) + PageCenter, pagesCount);
        }

        public static bool GetNeedEndPoints(int selectedPage, int pagesCount)
        {
            return GetEndPage(selectedPage, pagesCount) != pagesCount;
        }
    }
}