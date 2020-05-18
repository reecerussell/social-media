using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Core.Abstractions
{
    internal class SecretProvider : ISecretProvider
    {
        private readonly IAmazonSecretsManager _client;

        public SecretProvider()
        {
            _client = new AmazonSecretsManagerClient();
        }

        public async Task<RSA> GetRsa(string name, string key)
        {
            string secret;
            var response = await _client.GetSecretValueAsync(new GetSecretValueRequest {SecretId = name});
            if (response.SecretString != null)
            {
                secret = response.SecretString;
            }
            else
            {
                await using var memoryStream = response.SecretBinary;
                using var reader = new StreamReader(memoryStream);
                secret = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
            }

            var content = JsonConvert.DeserializeObject<IDictionary<string, string>>(secret);
            using var keyReader = new StringReader(content[key]);
            var publicKeyParam = (RsaKeyParameters)new PemReader(keyReader).ReadObject();

            return RSA.Create(new RSAParameters
            {
                Modulus = publicKeyParam.Modulus.ToByteArrayUnsigned(),
                Exponent = publicKeyParam.Exponent.ToByteArrayUnsigned()
            });
        }

        public async Task<string> GetMySqlConnectionStringAsync(string name)
        {
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
                secret = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
            }

            var content = JsonConvert.DeserializeObject<IDictionary<string, string>>(secret);

            return
                $"Database={content["dbname"]};Data Source={content["host"]};User Id={content["username"]};Password={content["password"]};respect binary flags=False;";
        }
    }
}
