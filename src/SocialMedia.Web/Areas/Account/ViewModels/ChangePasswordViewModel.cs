namespace SocialMedia.Web.Areas.Account.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string Error { get; set; }
    }
}
