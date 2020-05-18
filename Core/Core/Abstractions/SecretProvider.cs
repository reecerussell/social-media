using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;

namespace Core.Abstractions
{
    internal class SecretProvider : ISecretProvider
    {
        private readonly IAmazonSecretsManager _client;

        public SecretProvider(IAmazonSecretsManager client)
        {
            _client = client;
        }

        public async Task<RSA> GetRsa(string name, string key)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

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
                secret = Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
            }

            var content = JsonConvert.DeserializeObject<IDictionary<string, string>>(secret);
            using var keyReader = new StringReader(FormatRsaData(content[key]));
            var asymmetricKeyPair = (AsymmetricCipherKeyPair)new PemReader(keyReader).ReadObject();
            var privateKeyParams = (RsaPrivateCrtKeyParameters) asymmetricKeyPair.Private;

            return RSA.Create(new RSAParameters
            {
                Modulus = privateKeyParams.Modulus.ToByteArrayUnsigned(),
                P = privateKeyParams.P.ToByteArrayUnsigned(),
                Q = privateKeyParams.Q.ToByteArrayUnsigned(),
                DP = privateKeyParams.DP.ToByteArrayUnsigned(),
                DQ = privateKeyParams.DQ.ToByteArrayUnsigned(),
                InverseQ = privateKeyParams.QInv.ToByteArrayUnsigned(),
                D = privateKeyParams.Exponent.ToByteArrayUnsigned(),
                Exponent = privateKeyParams.PublicExponent.ToByteArrayUnsigned()
            });
        }

        private static string FormatRsaData(string content)
        {
            var parts = content.Split(" ");
            var partCount = parts.Length;

            var data = string.Join(" ", parts[Range.EndAt(4)]);

            for (var i = 4; i < partCount - 4; i++)
            {
                data += "\n" + parts[i];
            }

            return data + "\n" + string.Join(" ", parts[Range.StartAt(partCount - 4)]);
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
                secret = Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
            }

            var content = JsonConvert.DeserializeObject<IDictionary<string, string>>(secret);

            return
                $"Database={content["dbname"]};Data Source={content["host"]};User Id={content["username"]};Password={content["password"]};respect binary flags=False;";
        }
    }
}
