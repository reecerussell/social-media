using CSharpFunctionalExtensions;
using System;

namespace Core.Models
{
    public class PostLike
    {
        public string PostId { get; private set; }
        public string UserId { get; private set; }
        public DateTime Date { get; private set; }

        private PostLike()
        {
        }

        private PostLike(string postId, string userId)
        {
            if (string.IsNullOrEmpty(postId))
            {
                throw new ArgumentNullException(nameof(postId));
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            PostId = postId;
            UserId = userId;
            Date = DateTime.Now;
        }

        internal static Result<PostLike> Create(Post post, User user)
        {
            return Result.Ok(new PostLike(post.Id, user.Id));
        }
    }
}
