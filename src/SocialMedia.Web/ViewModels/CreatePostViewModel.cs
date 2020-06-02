using SocialMedia.Core.Dtos;

namespace SocialMedia.Web.ViewModels
{
    public class CreatePostViewModel
    {
        public string Error { get; set; }
        public CreatePostDto Post { get; set; }
    }
}
