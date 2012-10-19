// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Controllers
{
    using System.Web.Mvc;

    using OutcoldSolutions.Web.Blog.Filters;
    using OutcoldSolutions.Web.Blog.Models;

    using WebMatrix.WebData;

    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult LogOn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return this.View("LogOn");
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return this.RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError(string.Empty, "The user name or password provided is incorrect.");
            return this.View("LogOn", model);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }
            else
            {
                return this.RedirectToAction("Index", "Site");
            }
        }
    }
}
