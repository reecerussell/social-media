using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace SocialMedia.Auth.Filters
{
    public static class AuthorizationFilter
    {
        public static AuthorizeFilter Build()
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

            return new AuthorizeFilter(policy);
        }
    }
}
