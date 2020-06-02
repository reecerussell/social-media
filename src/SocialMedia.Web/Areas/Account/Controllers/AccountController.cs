using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Auth;
using SocialMedia.Core;
using SocialMedia.Core.Dtos;
using SocialMedia.Web.Areas.Account.ViewModels;
using System.Threading.Tasks;

namespace SocialMedia.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class AccountController : Controller
    {
        private readonly IUserProvider _userProvider;
        private readonly IUserManager _userManager;

        private const string AccountErrorKey = "account:error";

        public AccountController(
            IUserProvider userProvider,
            IUserManager userManager)
        {
            _userProvider = userProvider;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var accountOrNothing = await _userProvider.GetUserAccountAsync();
            if (accountOrNothing.HasNoValue)
            {
                return NotFound();
            }

            var viewModel = new AccountViewModel
            {
                Account = accountOrNothing.Value
            };

            if (TempData[AccountErrorKey] is string error)
            {
                viewModel.Error = error;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AccountEditViewModel model)
        {
            var updateDto = new UpdateUserDto
            {
                Username = model.Username,
                Bio = model.Bio,
                Socials = model.Socials
            };

            var (success, _, error) = await _userManager.UpdateAsync(updateDto);
            if (!success)
            {
                TempData[AccountErrorKey] = error;

                return RedirectToAction(nameof(Index));
            }

            if (model.File != null && model.File.Length > 0)
            {
                var updateMediaDto = new UpdateMediaDto
                {
                    Description = model.Username,
                    File = model.File
                };

                (success, _, error) = await _userManager.ChangeProfilePictureAsync(updateMediaDto);
                if (!success)
                {
                    TempData[AccountErrorKey] = error;

                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
