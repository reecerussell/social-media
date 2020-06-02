using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Core;
using SocialMedia.Core.EntityFramework;
using SocialMedia.Domain.Models;
using System.Threading.Tasks;

namespace SocialMedia.Media.Repositories
{
    internal class MimeTypeRepository : BaseRepository<MimeType>
    {
        private readonly INormalizer _normalizer;

        public MimeTypeRepository(
            MediaContext context, 
            ILogger<BaseRepository<MimeType>> logger,
            INormalizer normalizer) 
            : base(context, logger)
        {
            _normalizer = normalizer;
        }

        public async Task<Maybe<MimeType>> FindByNameAsync(string name)
        {
            return await Set
                .SingleOrDefaultAsync(x => x.NormalizedName == _normalizer.Normalize(name));
        }
    }
}
