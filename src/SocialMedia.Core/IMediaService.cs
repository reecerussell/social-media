using CSharpFunctionalExtensions;
using SocialMedia.Core.Dtos;
using System.Threading.Tasks;

namespace SocialMedia.Core
{
    public interface IMediaService
    {
        Task<Result<(byte[] Data, string ContentType)>> DownloadAsync(string id);
        Task<Result<string>> CreateAsync(UpdateMediaDto media);
        Task<Result> DeleteAsync(string id);
        Task<Result> DeleteMediaForUserAsync(string userId);
    }
}
