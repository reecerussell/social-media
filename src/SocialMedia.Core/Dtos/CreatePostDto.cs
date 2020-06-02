using Microsoft.AspNetCore.Http;

namespace SocialMedia.Core.Dtos
{
    public class CreatePostDto
    {
        public string Caption { get; set; }
        public IFormFile File { get; set; }
    }
}
