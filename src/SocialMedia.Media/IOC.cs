using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using SocialMedia.Core;
using SocialMedia.Media.Repositories;

namespace SocialMedia.Media
{
    public static class Ioc
    {
        public static IServiceCollection AddMedia(this IServiceCollection services)
        {
            return services
                .AddAWSService<IAmazonS3>()
                .AddScoped<MediaContext>()
                .AddTransient<MimeTypeRepository>()
                .AddTransient<MediaRepository>()
                .AddTransient<IMediaService, MediaService>();
        }
    }
}
