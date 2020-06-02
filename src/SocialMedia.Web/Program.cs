using Amazon.S3;
using Amazon.SQS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SocialMedia.Core.Logging;

namespace SocialMedia.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddProvider(BuildLoggerProvider(logging.Services));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static ILoggerProvider BuildLoggerProvider(IServiceCollection services)
        {
            var provider = services
                .AddAWSService<IAmazonSQS>()
                .BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();
            var awsClient = provider.GetRequiredService<IAmazonSQS>();

            return new LoggerProvider(configuration, awsClient);
        }
    }
}
