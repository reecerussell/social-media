using Core.Models;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public class MediaRepository
    {
        private readonly CoreDbContext _context;

        private DbSet<Media> _media;
        private DbSet<Media> Media => _media ??= _context.Set<Media>();

        public MediaRepository(
            CoreDbContext context)
        {
            _context = context;
        }

        public async Task<Maybe<Media>> FindByIdAsync(string id)
        {
            return await Media.FindAsync(id);
        }

        public async Task<Maybe<Media>> FindByIdAsNoTrackingAsync(string id)
        {
            return await Media
                .AsNoTracking()
                .Include(x => x.MimeType)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Add(Media media)
        {
            Media.Add(media);
        }

        public void Remove(Media media)
        {
            Media.Remove(media);
        }

        public async Task<Result> SaveChangesAsync()
        {
            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}