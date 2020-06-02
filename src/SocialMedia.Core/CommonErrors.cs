namespace SocialMedia.Core
{
    public class CommonErrors
    {
        public const string UserNotFound = "User not found.";
        public const string PostNotFound = "Post not found.";
        public const string MediaNotFound = "Media not found.";

        public static bool IsNotFound(string error)
        {
            return error switch
            {
                UserNotFound => true,
                PostNotFound => true,
                MediaNotFound => true,
                _ => false
            };
        }
    }
}
