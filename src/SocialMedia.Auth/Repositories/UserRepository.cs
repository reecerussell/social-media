using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Core;
using SocialMedia.Core.EntityFramework;
using SocialMedia.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Auth.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        private readonly INormalizer _normalizer;

        public UserRepository(
            UserContext context,
            ILogger<UserRepository> logger,
            INormalizer normalizer) 
            : base(context, logger)
        {
            _normalizer = normalizer;
        }

        public async Task<Maybe<User>> FindByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            return await Set
                .Include(c => c.LoginAttempts)
                .SingleOrDefaultAsync(x => x.Username == _normalizer.Normalize(username));
        }

        public async Task<Maybe<User>> FindByIdWithFollowersAsync(string id)
        {
            return await Set
                .Include(x => x.Followers)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Maybe<User>> FindByIdWithRelatedDataAsync(string id)
        {
            return await Set
                .Include(x => x.ProfileImage)
                .Include(x => x.Posts)
                    .ThenInclude(x => x.Media)
                .Include(x => x.Followers)
                .Include(x => x.LoginAttempts)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> ExistsWithUsernameAsync(string username, params string[] idsToIgnore)
        {
            return await Set
                .AsNoTracking()
                .Where(x => idsToIgnore.All(y => y != x.Id))
                .AnyAsync(
                    x => x.NormalizedUsername == _normalizer.Normalize(username));
        }
    }
}
