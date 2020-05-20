using Core.Dtos;
using CSharpFunctionalExtensions;
using System.Threading.Tasks;

namespace Core
{
    public interface IMediaDownloader
    {
        Task<Result<DownloadMediaDto>> DownloadAsync(IContext context, string id);
    }
}
