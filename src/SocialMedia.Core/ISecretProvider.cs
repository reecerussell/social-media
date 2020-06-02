using System.Threading.Tasks;

namespace SocialMedia.Core
{
    public interface ISecretProvider
    {
        Task<string> GetMySqlConnectionStringAsync(string name);
    }
}
