using CSharpFunctionalExtensions;
using SocialMedia.Core.Dtos;
using System.Threading.Tasks;

namespace SocialMedia.Core
{
    public interface IPostManager
    {
        Task<Result<string>> CreateAsync(CreatePostDto dto);
        Task<Result> DeleteAsync(string postId);
        Task<Result> AddCommentAsync(CreateCommentDto dto);
        Task<Result> DeleteCommentAsync(string postId, string commentId);
        Task<Result> LikeAsync(string postId);
        Task<Result> UnlikeAsync(string postId);
    }
}
