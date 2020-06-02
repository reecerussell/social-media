namespace SocialMedia.Auth.Dtos
{
    public class LoggedInUserDto
    {
        public string Username { get; set; }
        public string ProfileImageId { get; set; }
        public int FollowerCount { get; set; }
    }
}
