namespace Core
{
    public class CommonErrors
    {
        public const string UserNotFound = "User coud not be found";
        public const string PostNotFound = "Post could not be found";
        public const string MediaNotFound = "Media could not be found";

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
