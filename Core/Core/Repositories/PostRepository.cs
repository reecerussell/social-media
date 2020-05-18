using Core.Models;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public class PostRepository
    {
        private readonly CoreDbContext _context;

        private DbSet<Post> _posts;
        private DbSet<Post> Posts => _posts ??= _context.Set<Post>();

        public PostRepository(
            CoreDbContext context)
        {
            _context = context;
        }

        public async Task<Maybe<Post>> FindByIdAsync(string id)
        {
            return await Posts.FindAsync(id);
        }

        public void Add(Post post)
        {
            Posts.Add(post);
        }

        public void Remove(Post posts)
        {
            Posts.Remove(posts);
        }

        public async Task<Result> SaveChangesAsync()
        {
            await _context.SaveChangesAsync();

            return Result.Ok();
        }
    }
}
