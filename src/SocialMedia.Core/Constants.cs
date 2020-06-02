namespace SocialMedia.Core
{
    public class Constants
    {
        // Password config
        public const string PasswordIterationCountKey = "Password:IterationCount";
        public const string PasswordSaltSizeKey = "Password:SaltSize";
        public const string PasswordKeySizeKey = "Password:KeySize";

        // Authentication config
        public const string LoginPathKey = "Authentication:LoginPath";
        public const string LogoutPathKey = "Authentication:LogoutPath";
        public const string ReturnUrlKey = "Authentication:ReturnUrlParameter";
        public const string CookieExpirationMinutesKey = "Authentication:CookieExpirationMinutes";
        public const string CookieNameKey = "Authentication:CookieName";

        public const string BasePageTitleKey = "Web:BasePageTitle";
        public const string BackgroundImageUrlKey = "Web:BasegroundImageUrl";
        public const string AntiForgeryFormFieldNameKey = "Web:AntiForgery:FormFieldName";
        public const string AntiForgeryCookieNameKey = "Web:AntiForgery:CookieName";
        public const string AntiForgeryHeaderNameKey = "Web:AntiForgery:HeaderName";

        // Media config
        public const string MediaBucketName = "Media:BucketName";

        // Logging config
        public const string LogQueueUrl = "Logging:LogQueueUrl";
    }
}
