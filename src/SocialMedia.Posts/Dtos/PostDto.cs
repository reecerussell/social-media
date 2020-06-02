using System;
using System.Collections.Generic;

namespace SocialMedia.Posts.Dtos
{
    public class PostDto
    {
        public string PostId { get; set; }
        public string PostMediaId { get; set; }
        public DateTime DateCreated { get; set; }
        public string Caption { get; set; }
        public string Username { get; set; }
        public string UserProfileImageId { get; set; }
        public string UserId { get; set; }
        public int LikeCount { get; set; }
        public bool HasCurrentUserLiked { get; set; }

        public IReadOnlyList<CommentDto> Comments { get; set; }
    }
}
