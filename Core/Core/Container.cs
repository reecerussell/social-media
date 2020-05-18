using Amazon.S3;
using Amazon.SecretsManager;
using Core.Abstractions;
using Core.Repositories;
using Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Core
{
    public static class Container
    {
        public static IServiceProvider Build()
        {
            var configBuilder = new ConfigurationBuilder();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
            if (environment.Equals("test", StringComparison.InvariantCultureIgnoreCase))
            {
                configBuilder.AddJsonFile("appsettings.json");
            }


            var configuration = configBuilder.Build();

            var services = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddDefaultAWSOptions(configuration.GetAWSOptions())
                .AddAWSService<IAmazonS3>()
                .AddAWSService<IAmazonSecretsManager>()
                .AddTransient<INormalizer, Normalizer>()
                .AddScoped<IPasswordHasher, PasswordHasher>()
                .AddSingleton<ISecretProvider, SecretProvider>()
                .AddScoped<CoreDbContext>()
                .AddScoped<MediaRepository>()
                .AddScoped<MimeTypeRepository>()
                .AddScoped<PostRepository>()
                .AddScoped<UserRepository>()
                .AddScoped<MediaService>()
                .AddTransient<IPostManager, PostService>()
                .AddTransient<IUserManager, UserService>();

            return services.BuildServiceProvider();
        }
    }
}
