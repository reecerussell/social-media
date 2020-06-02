using System;

namespace SocialMedia.Posts.Dtos
{
    public class CommentDto
    {
        public string CommentId { get; set; }
        public string ReplyToId { get; set; }
        public DateTime DateCreated { get; set; }
        public string Text { get; set; }
        public string Username { get; set; }
        public string UserProfileImageId { get; set; }
    }
}
