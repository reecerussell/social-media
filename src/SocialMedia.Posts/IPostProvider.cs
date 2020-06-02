using CSharpFunctionalExtensions;
using SocialMedia.Core.Dtos;
using SocialMedia.Posts.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialMedia.Posts
{
    public interface IPostProvider
    {
        Task<Maybe<PostDto>> GetPostAsync(string id);
        Task<IReadOnlyList<FeedItemDto>> GetFeedAsync();
    }
}
