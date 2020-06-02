using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Core.EntityFramework;
using SocialMedia.Domain.Models;
using System.Threading.Tasks;

namespace SocialMedia.Posts.Repository
{
    internal class PostRepository : BaseRepository<Post>
    {
        public PostRepository(
            PostContext context, 
            ILogger<BaseRepository<Post>> logger) 
            : base(context, logger)
        {
        }

        public async Task<Maybe<Post>> FindByIdWithCommentsAsync(string id)
        {
            return await Set
                .Include(x => x.Comments)
                    .ThenInclude(x => x.Replies)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Maybe<Post>> FindByIdWithLikesAsync(string id)
        {
            return await Set
                .Include(x => x.Likes)
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}
