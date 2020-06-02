using System;

namespace SocialMedia.Core.Dtos
{
    public class FeedItemDto
    {
        public string PostId { get; set; }
        public string PostMediaId { get; set; }
        public DateTime DateCreated { get; set; }
        public string Caption { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string UserProfileImageId { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }
}
