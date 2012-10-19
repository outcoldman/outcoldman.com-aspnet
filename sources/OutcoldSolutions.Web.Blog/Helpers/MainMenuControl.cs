// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Helpers
{
    using System.Collections.Generic;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;

    using OutcoldSolutions.Web.Blog.Resources;

    public class MainMenu
    {
        public MainMenu(string title, string action, string controller)
        {
            this.Action = action;
            this.Controller = controller;
            this.Title = title;
        }

        public string Action { get; set; }

        public string Controller { get; set; }

        public string Title { get; set; }

        public bool IsSelectByController { get; set; }
    }

    public static class MainMenuControl
    {
        public static MvcHtmlString CreateMainMenu(this HtmlHelper helper)
        {
            IList<MainMenu> menu = new List<MainMenu>
                {
                    new MainMenu(
                        helper.GetResource("HomePage").ToUpper(), "index", "site"), 
                    new MainMenu(
                        helper.GetResource("Blog").ToUpper(), "index", "blog")
                        {
                            IsSelectByController
                                =
                                true
                        }, 
                    new MainMenu(
                        helper.GetResource("Tools").ToUpper(), "index", "tools")
                        {
                            IsSelectByController
                                =
                                true
                        }, 
                    new MainMenu(
                        helper.GetResource("AboutMe").ToUpper(), "aboutme", "site"), 
                    new MainMenu(
                        helper.GetResource("Contacts").ToUpper(), "links", "site"), 
                    new MainMenu(
                        helper.GetResource("Search").ToUpper(), "search", "site")
                };
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<ul class='topNav'>");

            string currentAction = helper.GetAction();
            string currentController = helper.GetController();

            foreach (MainMenu item in menu)
            {
                stringBuilder.AppendFormat("<li>");
                Dictionary<string, object> attributes = new Dictionary<string, object>();

                if (item.Controller == currentController && (item.Action == currentAction || item.IsSelectByController))
                {
                    attributes.Add("class", "topNavAct");
                }

                stringBuilder.AppendFormat(
                    helper.ActionLink(
                        item.Title, 
                        item.Action, 
                        item.Controller, 
                        new RouteValueDictionary { { "id", string.Empty }, { "lang", helper.GetLanguage() } }, 
                        attributes).ToHtmlString());
                stringBuilder.AppendFormat("</li>");
            }

            stringBuilder.AppendLine("</ul>");
            return MvcHtmlString.Create(stringBuilder.ToString());
        }
    }
}