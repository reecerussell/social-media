using SocialMedia.Core.Dtos;

namespace SocialMedia.Web.ViewModels
{
    public class RegisterViewModel
    {
        public RegisterUserDto User { get; set; }
        public string Error { get; set; }
        public string ReturnUrl { get; set; }
    }
}
