using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SocialMedia.Auth;
using SocialMedia.Auth.Filters;
using SocialMedia.Core;
using SocialMedia.Media;
using SocialMedia.Posts;

namespace SocialMedia.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddCore()
                .AddMedia()
                .AddAuth(Configuration)
                .AddPosts();

            services.AddAntiforgery(options =>
            {
                options.FormFieldName = Configuration[Constants.AntiForgeryFormFieldNameKey];
                options.Cookie.Name = Configuration[Constants.AntiForgeryCookieNameKey];
                options.HeaderName = Configuration[Constants.AntiForgeryHeaderNameKey];
            });

            services
                .AddControllersWithViews(options =>
                    options.Filters.Add(AuthorizationFilter.Build()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Lax
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "accountDefault",
                    areaName: "Account",
                    pattern: "Account/{controller=Account}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Feed}/{action=Index}/{id?}");
            });
        }
    }
}
