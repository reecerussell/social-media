using Core.Models;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public class MimeTypeRepository
    {
        private readonly CoreDbContext _context;
        private readonly INormalizer _normalizer;

        private DbSet<MimeType> _mimeTypes;
        private DbSet<MimeType> MimeTypes => _mimeTypes ??= _context.Set<MimeType>();

        public MimeTypeRepository(
            CoreDbContext context,
            INormalizer normalizer)
        {
            _context = context;
            _normalizer = normalizer;
        }

        public async Task<Maybe<MimeType>> FindByNameAsync(string name)
        {
            return await MimeTypes.FirstOrDefaultAsync(x => x.NormalizedName == _normalizer.Normalize(name));
        }

        public void Add(MimeType mimeType)
        {
            MimeTypes.Add(mimeType);
        }

        public async Task<Result> SaveChangesAsync()
        {
            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}
