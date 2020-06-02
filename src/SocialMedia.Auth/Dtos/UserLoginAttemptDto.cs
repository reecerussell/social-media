using System;

namespace SocialMedia.Auth.Dtos
{
    public class UserLoginAttemptDto
    {
        public string RemoteAddress { get; set; }
        public bool Successful { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}
