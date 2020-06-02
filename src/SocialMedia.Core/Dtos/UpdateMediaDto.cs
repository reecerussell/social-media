using Microsoft.AspNetCore.Http;

namespace SocialMedia.Core.Dtos
{
    public class UpdateMediaDto
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public IFormFile File { get; set; }
    }
}
