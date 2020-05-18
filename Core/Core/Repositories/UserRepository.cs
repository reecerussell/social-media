using Core.Models;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public class UserRepository
    {
        private readonly CoreDbContext _context;
        private readonly INormalizer _normalizer;

        private DbSet<User> _users;
        private DbSet<User> Users => _users ??= _context.Set<User>();

        public UserRepository(
            CoreDbContext context,
            INormalizer normalizer)
        {
            _context = context;
            _normalizer = normalizer;
        }

        public async Task<Maybe<User>> FindByIdAsync(string id)
        {
            return await Users.FindAsync(id);
        }

        public async Task<Maybe<User>> FindByUsernameAsync(string username)
        {
            return await Users.FirstOrDefaultAsync(
                x => x.NormalizedUsername == _normalizer.Normalize(username));
        }

        public async Task<bool> ExistsWithUsernameAsync(string username)
        {
            return await Users
                .AsNoTracking()
                .AnyAsync(
                x => x.NormalizedUsername == _normalizer.Normalize(username));
        }

        public void Add(User user)
        {
            Users.Add(user);
        }

        public void Remove(User user)
        {
            Users.Remove(user);
        }

        public async Task<Result> SaveChangesAsync()
        {
            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}
