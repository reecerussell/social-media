using Amazon.SecretsManager;
using Microsoft.Extensions.DependencyInjection;

namespace SocialMedia.Core
{
    public static class Ioc
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            return services
                .AddAWSService<IAmazonSecretsManager>()
                .AddTransient<INormalizer, Normalizer>()
                .AddTransient<IConnectionStringProvider, ConnectionStringProvider>()
                .AddTransient<ISecretProvider, SecretProvider>();
        }
    }
}
