namespace SocialMedia.Core
{
    public interface IUser
    {
        string Id { get; }
        string Username { get; }
        int FollowerCount { get; }
        string ProfileImageId { get; }
    }
}
