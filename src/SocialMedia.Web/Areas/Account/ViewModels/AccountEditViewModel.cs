using Microsoft.AspNetCore.Http;
using SocialMedia.Core.Dtos;

namespace SocialMedia.Web.Areas.Account.ViewModels
{
    public class AccountEditViewModel
    {
        public string Username { get; set; }
        public string Bio { get; set; }
        public UpdateSocialsDto Socials { get; set; }
        public IFormFile File { get; set; }
    }
}
