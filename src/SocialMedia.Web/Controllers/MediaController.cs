using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Core;
using System.IO;
using System.Threading.Tasks;

namespace SocialMedia.Web.Controllers
{
    public class MediaController : Controller
    {
        private readonly IMediaService _mediaService;

        public MediaController(
            IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        public async Task<IActionResult> Download(string id)
        {
            var (success, _, (data, contentType), error) = await _mediaService.DownloadAsync(id);
            if (!success)
            {
                if (CommonErrors.IsNotFound(error))
                {
                    return NotFound();
                }

                return BadRequest(error);
            }

            var stream = new MemoryStream(data);

            return File(stream, contentType);
        }
    }
}
