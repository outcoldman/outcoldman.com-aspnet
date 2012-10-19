namespace OutcoldSolutions.Web.Blog.Controllers
{
    using System.Web.Mvc;

    public class ShowIPController : Controller
    {
        public ActionResult UserIP()
        {
            return this.View();
        }
    }
}