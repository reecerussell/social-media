using System.Threading.Tasks;

namespace SocialMedia.Core
{
    public interface IConnectionStringProvider
    {
        Task<string> GetConnectionStringAsync();
    }
}
