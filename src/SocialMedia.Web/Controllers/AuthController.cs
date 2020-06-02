using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Core;
using SocialMedia.Core.Dtos;
using SocialMedia.Web.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SocialMedia.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserManager _userManager;

        public AuthController(
            IUserManager userManager)
        {
            _userManager = userManager;
        }

        [Route("Login")]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel
            {
                User = new LoginUserDto(), 
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var (success, _, error) = await _userManager.LoginAsync(model.User);
            if (!success)
            {
                model.Error = error;

                return View(model);
            }

            if (!string.IsNullOrEmpty(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return Redirect("/");
        }

        [Route("Register")]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl)
        {
            return View(new RegisterViewModel
            {
                User = new RegisterUserDto(),
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var (success, _, error) = await _userManager.RegisterAsync(model.User);
            if (!success)
            {
                model.Error = error;

                return View(model);
            }

            if (!string.IsNullOrEmpty(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return Redirect("/");
        }

        [Route("Logout")]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await Request.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (string.IsNullOrEmpty(returnUrl))
            {
                return Redirect("/");
            }

            return Redirect(returnUrl);
        }
    }
}
