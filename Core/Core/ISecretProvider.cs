using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Core
{
    public interface ISecretProvider
    {
        Task<RSA> GetRsa(string name, string key);
        Task<string> GetMySqlConnectionStringAsync(string name);
    }
}
