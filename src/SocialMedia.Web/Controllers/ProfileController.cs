using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Auth;
using SocialMedia.Core;
using System.Threading.Tasks;

namespace SocialMedia.Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUserProvider _userProvider;
        private readonly IUserManager _userManager;
        
        public ProfileController(
            IUserProvider userProvider,
            IUserManager userManager)
        {
            _userProvider = userProvider;
            _userManager = userManager;
        }

        [Route("{username}", Name = "profile", Order = 10000)]
        public async Task<IActionResult> Index(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return Redirect("/");
            }

            var profileOrNothing = await _userProvider.GetProfileAsync(username);
            if (profileOrNothing.HasNoValue)
            {
                return NotFound();
            }

            return View(profileOrNothing.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Follow(string userId, string username)
        {
            var (success, _, error) = await _userManager.FollowAsync(userId);
            if (!success)
            {
                return BadRequest(error);
            }

            return RedirectToRoute("profile", new {username});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unfollow(string userId, string username)
        {
            var (success, _, error) = await _userManager.UnfollowAsync(userId);
            if (!success)
            {
                return BadRequest(error);
            }

            return RedirectToRoute("profile", new { username });
        }
    }
}