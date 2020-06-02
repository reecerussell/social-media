using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialMedia.Auth.Providers;
using SocialMedia.Auth.Repositories;
using SocialMedia.Core;
using SocialMedia.Core.Extensions;
using System;

namespace SocialMedia.Auth
{
    public static class Ioc
    {
        public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddTransient<IPasswordHasher, PasswordHasher>()
                .AddScoped<UserContext>()
                .AddTransient<UserRepository>()
                .AddTransient<IUserManager, UserManager>()
                .AddTransient<IUserProvider, UserProvider>()
                .AddTransient<IHttpContextAccessor, HttpContextAccessor>()
                .AddTransient<IUser, CurrentUser>()
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.LoginPath = configuration[Constants.LoginPathKey];
                        options.LogoutPath = configuration[Constants.LogoutPathKey];
                        options.AccessDeniedPath = configuration[Constants.LoginPathKey];
                        options.ReturnUrlParameter = configuration[Constants.ReturnUrlKey];
                        options.ExpireTimeSpan = TimeSpan.FromMinutes(configuration.GetInt(
                            Constants.CookieExpirationMinutesKey, 60));
                        options.Cookie.Name = configuration[Constants.CookieNameKey];
                        options.Cookie.HttpOnly = true;
                        options.Cookie.SameSite = SameSiteMode.Lax;
                        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    });

            return services;
        }
    }
}
