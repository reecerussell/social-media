using SocialMedia.Auth.Dtos;

namespace SocialMedia.Web.Areas.Account.ViewModels
{
    public class AccountViewModel
    {
        public AccountDto Account { get; set; }
        public string Error { get; set; }
    }
}
