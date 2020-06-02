using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace SocialMedia.Core
{
    internal class ConnectionStringProvider : IConnectionStringProvider
    {
        private readonly ISecretProvider _secretProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConnectionStringProvider> _logger;

        public ConnectionStringProvider(
            ISecretProvider secretProvider,
            IConfiguration configuration,
            ILogger<ConnectionStringProvider> logger)
        {
            _secretProvider = secretProvider;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GetConnectionStringAsync()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?.ToUpper(CultureInfo.InvariantCulture);

            switch (environment)
            {
                case "DEVELOPMENT":
                    return await _secretProvider.GetMySqlConnectionStringAsync("dev/SocialMedia/mysql");
                case "PRODUCTION":
                    return await _secretProvider.GetMySqlConnectionStringAsync("prod/SocialMedia/mysql");
                case "TEST":
                    return _configuration["ConnectionStrings:DefaultConnection"];
                default:
                    var ex = new ArgumentException("No connection string is configured for this environment");
                    _logger.LogError(ex, ex.Message);
                    throw ex;
            }
        }
    }
}
