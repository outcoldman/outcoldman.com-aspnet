using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using System.Web.Security;
using PersonalWeb.Model;

namespace PersonalWeb.Controllers
{
	public class AccountController : Controller
	{
		public ActionResult LogOn()
		{
			return View();
		}

		[HttpPost]
		[SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings",
			Justification = "Needs to take same parameter type as Controller.Redirect()")]
		public ActionResult LogOn(LogOnModel model, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				if (Membership.Provider.ValidateUser(model.UserName, model.Password))
				{
					FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
					if (!String.IsNullOrEmpty(returnUrl))
					{
						return Redirect(returnUrl);
					}
					else
					{
						return RedirectToAction("index", "site");
					}
				}
				else
				{
					ModelState.AddModelError("", "The user name or password provided is incorrect.");
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}
	}
}