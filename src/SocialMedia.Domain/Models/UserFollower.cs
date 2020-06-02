using CSharpFunctionalExtensions;
using System;

namespace SocialMedia.Domain.Models
{
    public class UserFollower
    {
        public string UserId { get; private set; }
        public string FollowerId { get; private set; }

        private UserFollower()
        {
        }

        private UserFollower(string userId, string followerId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (string.IsNullOrEmpty(followerId))
            {
                throw new ArgumentNullException(nameof(followerId));
            }

            UserId = userId;
            FollowerId = followerId;
        }

        internal static Result<UserFollower> Create(string userId, string followerId)
        {
            return Result.Ok(new UserFollower(userId, followerId));
        }
    }

}
