using SocialMedia.Core.Dtos;

namespace SocialMedia.Web.ViewModels
{
    public class LoginViewModel
    {
        public LoginUserDto User { get; set; }
        public string Error { get; set; }
        public string ReturnUrl { get; set; }
    }
}
