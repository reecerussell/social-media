using Core.Dtos;
using CSharpFunctionalExtensions;
using System.Threading.Tasks;

namespace Core
{
    public interface IPostManager
    {
        Task<Result<string>> CreateAsync(IContext context, CreatePostDto dto);
        Task<Result> DeleteAsync(IContext context, string id);
        Task<Result> AddCommentAsync(IContext context, CreateCommentDto dto);
        Task<Result> DeleteCommentAsync(IContext context, string postId, string commentId);
        Task<Result> LikeAsync(IContext context, string id);
        Task<Result> UnlikeAsync(IContext context, string id);
    }
}
