using Core.Abstractions;
using Core.Repositories;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Configuration;

namespace Core
{
    public static class Container
    {
        public static IServiceProvider Build()
        {
            var services = new ServiceCollection()
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

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
            if (environment.Equals("test", StringComparison.InvariantCultureIgnoreCase))
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json");
                services.AddSingleton<IConfiguration>(builder.Build());
            }

            return services.BuildServiceProvider();
        }
    }
}
