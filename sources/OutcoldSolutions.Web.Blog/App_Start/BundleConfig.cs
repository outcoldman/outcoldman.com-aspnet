// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            bundles.Add(new ScriptBundle("~/bundles/jquery", "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.2.min.js")
                .Include("~/Scripts/jquery-1.8.2.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/main")
                .Include("~/Scripts/modernizr.js")
                .Include("~/Scripts/Master.js")
                .Include("~/Scripts/jquery.cookie.js"));

            bundles.Add(new ScriptBundle("~/bundles/itemview")
                .Include("~/Scripts/ItemView.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));
        }
    }
}