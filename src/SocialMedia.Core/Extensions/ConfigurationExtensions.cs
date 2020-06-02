using Microsoft.Extensions.Configuration;

namespace SocialMedia.Core.Extensions
{
    public static class ConfigurationExtensions
    {
        public static int GetInt(this IConfiguration configuration, string key, int defaultValue = default)
        {
            var value = configuration[key].Trim();
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            if (int.TryParse(value, out var n))
            {
                return n;
            }

            return defaultValue;
        }
    }
}
