using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia.Core
{
    internal class SecretProvider : ISecretProvider
    {
        private readonly IAmazonSecretsManager _client;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SecretProvider> _logger;

        private readonly EventId _getMySqlConnectionEventKey = new EventId(1, "Get MySql ConnectionString");

        public SecretProvider(
            IAmazonSecretsManager client,
            IMemoryCache cache,
            ILogger<SecretProvider> logger)
        {
            _client = client;
            _cache = cache;
            _logger = logger;
        }

        public async Task<string> GetMySqlConnectionStringAsync(string name)
        {
            if (_cache.TryGetValue<string>($"{name}_MySqlConnectionString", out var cachedValue))
            {
                _logger.LogInformation(_getMySqlConnectionEventKey, "Using cached MySql connection string for: " + name);
                return cachedValue;
            }

            _logger.LogInformation(_getMySqlConnectionEventKey, $"Requesting MySql credentials for secret '{name}'...");

            string secret;
            var response = await _client.GetSecretValueAsync(new GetSecretValueRequest { SecretId = name });
            if (response.SecretString != null)
            {
                secret = response.SecretString;
            }
            else
            {
                await using var memoryStream = response.SecretBinary;
                using var reader = new StreamReader(memoryStream);
                secret = Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
            }

            _logger.LogInformation(_getMySqlConnectionEventKey, $"Received MySql credentials for secret '{name}'.");

            var content = JsonConvert.DeserializeObject<IDictionary<string, string>>(secret);
            var connectionString = $"Database={content["dbname"]};" +
                                   $"Data Source={content["host"]};" +
                                   $"User Id={content["username"]};" +
                                   $"Password={content["password"]};" +
                                   "respect binary flags=False;";

            _logger.LogInformation(_getMySqlConnectionEventKey, $"Caching secret '{name}' with a sliding expiration of 5 minutes.");

            _cache.Set(
                $"{name}_MySqlConnectionString", 
                connectionString, 
                TimeSpan.FromMinutes(5));

            return connectionString;
        }
    }
}
