using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Core.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Media.Repositories
{
    internal class MediaRepository : BaseRepository<Domain.Models.Media>
    {
        public MediaRepository(
            MediaContext context, 
            ILogger<BaseRepository<Domain.Models.Media>> logger) 
            : base(context, logger)
        {
        }

        public async Task<Maybe<Domain.Models.Media>> FindByIdAsNoTrackingAsync(string id)
        {
            return await Set
                .AsNoTracking()
                .Include(x => x.MimeType)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<Domain.Models.Media>> FindByUserIdAsync(string userId)
        {
            return await Set
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }
    }
}
