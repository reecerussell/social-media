using Microsoft.Extensions.DependencyInjection;
using SocialMedia.Core;
using SocialMedia.Posts.Providers;
using SocialMedia.Posts.Repository;

namespace SocialMedia.Posts
{
    public static class Ioc
    {
        public static IServiceCollection AddPosts(this IServiceCollection services)
        {
            return services
                .AddScoped<PostContext>()
                .AddTransient<PostRepository>()
                .AddTransient<IPostManager, PostManager>()
                .AddTransient<IPostProvider, PostProvider>();
        }
    }
}
