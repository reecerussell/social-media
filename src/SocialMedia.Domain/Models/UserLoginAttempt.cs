using System;

namespace SocialMedia.Domain.Models
{
    public class UserLoginAttempt
    {
        public string Id { get; private set; }
        public string UserId { get; private set; }
        public string RemoteAddress { get; private set; }
        public bool Successful { get; private set; }
        public string Message { get; private set; }
        public DateTime Date { get; private set; }

        private UserLoginAttempt()
        {
        }

        private UserLoginAttempt(string userId, string remoteAddress, bool isSuccessful, string message)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrEmpty(remoteAddress)) throw new ArgumentNullException(nameof(remoteAddress));
            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));

            Id = Guid.NewGuid().ToString();
            UserId = userId;
            RemoteAddress = remoteAddress;
            Successful = isSuccessful;
            Message = message;
            Date = DateTime.UtcNow;
        }

        internal static UserLoginAttempt Success(string userId, string remoteAddress)
        {
            const string message = "A successful login attempt!";

            return new UserLoginAttempt(userId, remoteAddress, true, message);
        }

        internal static UserLoginAttempt TooManyFailedAttempts(string userId, string remoteAddress)
        {
            const string message = "Account locked due to too many failed login attempts.";

            return new UserLoginAttempt(userId, remoteAddress, false, message);
        }

        internal static UserLoginAttempt Locked(string userId, string remoteAddress)
        {
            const string message = "Failed to login due to account being locked.";

            return new UserLoginAttempt(userId, remoteAddress, false, message);
        }

        internal static UserLoginAttempt IncorrectPassword(string userId, string remoteAddress)
        {
            const string message = "Failed to login due to password being incorrect.";

            return new UserLoginAttempt(userId, remoteAddress, false, message);
        }
    }
}
