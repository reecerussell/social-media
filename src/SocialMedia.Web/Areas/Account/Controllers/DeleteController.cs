using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Core;
using SocialMedia.Web.Areas.Account.ViewModels;
using System.Threading.Tasks;

namespace SocialMedia.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class DeleteController : Controller
    {
        private readonly IUserManager _userManager;

        public DeleteController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        private const string DeleteErrorKey = "delete:error";

        public IActionResult Index()
        {
            var viewModel = new DeleteViewModel();

            if (TempData[DeleteErrorKey] is string error)
            {
                viewModel.Error = error;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> Delete()
        {
            var (success, _, error) = await _userManager.DeleteAsync();
            if (!success)
            {
                if (CommonErrors.IsNotFound(error))
                {
                    return NotFound();
                }

                TempData[DeleteErrorKey] = error;

                return RedirectToAction(nameof(Index));
            }

            await HttpContext.SignOutAsync();

            return RedirectToAction("Login", "Auth", new {area = ""});
        }
    }
}
