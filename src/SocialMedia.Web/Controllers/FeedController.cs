using Microsoft.AspNetCore.Mvc;
using SocialMedia.Posts;
using System.Threading.Tasks;

namespace SocialMedia.Web.Controllers
{
    public class FeedController : Controller
    {
        private readonly IPostProvider _provider;

        public FeedController(
            IPostProvider provider)
        {
            _provider = provider;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _provider.GetFeedAsync();

            return View(items);
        }
    }
}
