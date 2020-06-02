using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace SocialMedia.Web.Helpers
{
    public static class UrlHelper
    {
        public static string LogoutUrl(this IUrlHelper helper)
        {
            var url = helper.ActionContext.HttpContext.Request.GetEncodedUrl();
            return helper.Action("Logout", "Auth", new {returnUrl = url});
        }

        public static string ProfileUrl(this IUrlHelper helper, string username)
        {
            return helper.RouteUrl("profile", new {username});
        }
    }
}
