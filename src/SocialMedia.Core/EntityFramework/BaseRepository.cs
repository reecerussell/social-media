using System;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SocialMedia.Core.EntityFramework
{
    public abstract class BaseRepository<T>
        where T : class
    {
        private readonly BaseContext _context;
        private readonly ILogger<BaseRepository<T>> _logger;

        private DbSet<T> _set;
        protected DbSet<T> Set => _set ??= _set = _context.Set<T>();

        private readonly EventId _saveChangesEventId = new EventId(1, "SaveChangesAsync");
        
        protected BaseRepository(
            BaseContext context,
            ILogger<BaseRepository<T>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Maybe<T>> FindByIdAsync(string id)
        {
            return await Set.FindAsync(id);
        }

        public void Add(T item)
        {
            Set.Add(item);
        }

        public void Remove(T item)
        {
            Set.Remove(item);
        }

        public async Task<Result> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();

                return Result.Ok();
            }
            catch (DbUpdateConcurrencyException e)
            {
                const string message = "A concurrency error occured while updating database entries.";
                _logger.LogError(_saveChangesEventId, e, message);
                throw new Exception(message, e);
            }
            catch (DbUpdateException e)
            {
                const string message = "An error occured while updating database entries.";
                _logger.LogError(_saveChangesEventId, e, message);
                throw new Exception(message, e);
            }
        }
    }
}
