using SocialMedia.Core.Dtos;
using System.Collections.Generic;

namespace SocialMedia.Auth.Dtos
{
    public class ProfileDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string ProfileImageId { get; set; }
        public string Bio { get; set; }
        public string Snapchat { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
        public int FollowerCount { get; set; }
        public bool FollowedByUser { get; set; }

        public IReadOnlyList<FeedItemDto> Posts { get; set; }
    }
}
