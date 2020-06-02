using Microsoft.AspNetCore.Mvc;
using SocialMedia.Auth;
using System.Threading.Tasks;

namespace SocialMedia.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class LoginAttemptsController : Controller
    {
        private readonly IUserProvider _provider;

        public LoginAttemptsController(
            IUserProvider provider)
        {
            _provider = provider;
        }

        [Route("Account/LoginAttempts")]
        public async Task<IActionResult> Index()
        {
            return View(await _provider.GetUserLoginAttemptsAsync());
        }
    }
}
