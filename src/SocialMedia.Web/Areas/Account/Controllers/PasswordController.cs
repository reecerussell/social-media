using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Core;
using SocialMedia.Core.Dtos;
using SocialMedia.Web.Areas.Account.ViewModels;
using System.Threading.Tasks;

namespace SocialMedia.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class PasswordController : Controller
    {
        private readonly IUserManager _userManager;

        private const string ChangePasswordError = "changePassword:error";

        public PasswordController(
            IUserManager userManager)
        {
            _userManager = userManager;
        }

        [Route("Account/ChangePassword")]
        public IActionResult Change()
        {
            var viewModel = new ChangePasswordViewModel();

            if (TempData[ChangePasswordError] is string error)
            {
                viewModel.Error = error;
            }

            return View(viewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Account/ChangePassword")]
        public async Task<IActionResult> Change(ChangePasswordViewModel model)
        {
            if (model.NewPassword != model.ConfirmPassword)
            {
                TempData[ChangePasswordError] = "Your passwords don't match!";

                return RedirectToAction(nameof(Change));
            }

            var changePasswordDto = new ChangePasswordDto
            {
                CurrentPassword = model.CurrentPassword,
                NewPassword = model.NewPassword
            };

            var (success, _, error) = await _userManager.ChangePasswordAsync(changePasswordDto);
            if (!success)
            {
                if (CommonErrors.IsNotFound(error))
                {
                    return NotFound();
                }

                TempData[ChangePasswordError] = error;

                return RedirectToAction(nameof(Change));
            }

            return RedirectToAction("Index", "Account", new {area = "Account"});
        }
    }
}
