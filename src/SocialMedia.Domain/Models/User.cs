using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SocialMedia.Core;
using SocialMedia.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace SocialMedia.Domain.Models
{
    public class User
    {
        public string Id { get; private set; }
        public string Username { get; private set; }
        public string NormalizedUsername { get; private set; }
        public string PasswordHash { get; private set; }
        public string Bio { get; private set; }
        public bool LockedOut { get; private set; }
        public DateTime? LockoutEnd { get; private set; }
        public int AccessFailedCount { get; private set; }
        public string ProfileImageId { get; private set; }
        public string ConcurrencyStamp { get; private set; }
        public string Snapchat { get; private set; }
        public string Instagram { get; private set; }
        public string Twitter { get; private set; }

        private List<UserFollower> _followers;
        public virtual IReadOnlyList<UserFollower> Followers
        {
            get => _followers;
            set => _followers = (List<UserFollower>)value;
        }

        private List<UserLoginAttempt> _loginAttempts;
        public IReadOnlyList<UserLoginAttempt> LoginAttempts
        {
            get => _loginAttempts;
            set => _loginAttempts = (List<UserLoginAttempt>) value;
        }

        private List<Post> _posts;
        public IReadOnlyList<Post> Posts
        {
            get => _posts;
            set => _posts = (List<Post>) value;
        }

        public Media ProfileImage { get; private set; }

        private User(ILazyLoader lazyLoader)
        {
            // An EF Core constructor. LazyLoader is brought in to avoid
            // EF Core using the other constructor.
        }

        private User()
        {
            Id = Guid.NewGuid().ToString();
            ConcurrencyStamp = Guid.NewGuid().ToString();
        }

        public Result UpdateUsername(string username, INormalizer normalizer)
        {
            if (string.IsNullOrEmpty(username))
            {
                return Result.Failure("Username cannot be empty.");
            }

            if (username.Length > 25)
            {
                return Result.Failure("Username cannot be greater than 25 characters.");
            }

            if (username.Length < 3)
            {
                return Result.Failure("Username cannot be less than 3 characters.");
            }

            if (!Regex.IsMatch(username, "^[a-zA-Z0-9]+([._]?[a-zA-Z0-9]+)*$"))
            {
                return Result.Failure("Username can only contain letters, numbers, underscores and periods.");
            }

            Username = username;
            NormalizedUsername = normalizer.Normalize(username);
            ConcurrencyStamp = Guid.NewGuid().ToString();

            return Result.Ok();
        }

        public Result UpdateBio(string bio)
        {
            if (string.IsNullOrEmpty(bio))
            {
                Bio = null;

                return Result.Ok();
            }

            if (bio.Length > 255)
            {
                return Result.Failure("Bio cannot be greater than 255 characters.");
            }

            Bio = bio;
            ConcurrencyStamp = Guid.NewGuid().ToString();

            return Result.Ok();
        }

        public Result Update(UpdateUserDto dto, INormalizer normalizer)
        {
            return UpdateUsername(dto.Username, normalizer)
                .Bind(() => UpdateBio(dto.Bio))
                .Bind(() => UpdateSocials(dto.Socials));
        }

        public Result UpdatePassword(ChangePasswordDto dto, IPasswordHasher hasher)
        {
            if (!hasher.Verify(PasswordHash, dto.CurrentPassword))
            {
                return Result.Failure("Current Password is invalid.");
            }

            return SetPassword(dto.NewPassword, hasher);
        }

        private Result SetPassword(string password, IPasswordHasher hasher)
        {
            if (string.IsNullOrEmpty(password))
            {
                return Result.Failure("Password cannot be empty.");
            }

            if (password.Length < 6)
            {
                return Result.Failure("Password cannot be less than 6 characters.");
            }

            // Avoids giant passwords.
            if (password.Length > 256)
            {
                return Result.Failure("Password cannot be greater than 256 characters.");
            }

            PasswordHash = hasher.Hash(password);
            ConcurrencyStamp = Guid.NewGuid().ToString();

            return Result.Ok();
        }

        public Result CanLogin(string password, IPasswordHasher hasher, HttpContext context)
        {
            if (LockedOut && LockoutEnd != null)
            {
                if (LockoutEnd < DateTime.Now)
                {
                    LockedOut = true;
                    LockoutEnd = null;
                }
            }

            if (LockedOut)
            {
                if (AccessFailedCount >= 3)
                {
                    IncrementAccessFailedCount();
                    _loginAttempts.Add(UserLoginAttempt.TooManyFailedAttempts(Id, context.Connection.RemoteIpAddress.ToString()));
                    return Result.Failure("Your account has been locked after too many failed login attempts.");
                }

                IncrementAccessFailedCount();
                _loginAttempts.Add(UserLoginAttempt.Locked(Id, context.Connection.RemoteIpAddress.ToString()));
                return Result.Failure("Your account is currently locked.");
            }

            if (!hasher.Verify(PasswordHash, password))
            {
                IncrementAccessFailedCount();
                _loginAttempts.Add(UserLoginAttempt.IncorrectPassword(Id, context.Connection.RemoteIpAddress.ToString()));
                return Result.Failure("Your password is incorrect.");
            }

            AccessFailedCount = 0;
            _loginAttempts.Add(UserLoginAttempt.Success(Id, context.Connection.RemoteIpAddress.ToString()));

            return Result.Ok();

            void IncrementAccessFailedCount()
            {
                AccessFailedCount++;
                ConcurrencyStamp = Guid.NewGuid().ToString();

                if (AccessFailedCount >= 3)
                {
                    LockedOut = true;
                    LockoutEnd = DateTime.Now.AddHours(1);
                }
            }
        }

        public void SetProfilePicture(string mediaId)
        {
            ProfileImageId = mediaId;
            ConcurrencyStamp = Guid.NewGuid().ToString();
        }

        public Result AddFollower(User follower)
        {
            if (Followers.Any(x => x.FollowerId == follower.Id))
            {
                // The current user is already following this user.
                return Result.Failure("You're already following this user.");
            }

            return UserFollower.Create(Id, follower.Id)
                .Tap(uf => _followers.Add(uf));
        }

        public Result RemoveFollower(User follower)
        {
            var userFollower = Followers.SingleOrDefault(x => x.FollowerId == follower.Id);
            if (userFollower == null)
            {
                // The current user is not following this user.
                return Result.Failure("You're already not following this user.");
            }

            _followers.Remove(userFollower);

            return Result.Ok();
        }

        public Result Delete(IUser user)
        {
            if (user.Id != Id)
            {
                return Result.Failure("You're not allowed to delete this user!");
            }

            var postCount = Posts.Count;
            for (var i = postCount-1; i >= 0; i--)
            {
                _posts.Remove(Posts[i]);
            }

            var followerCount = Posts.Count;
            for (var i = followerCount-1; i >= 0; i--)
            {
                _followers.Remove(Followers[i]);
            }

            var loginCount = LoginAttempts.Count;
            for (var i = loginCount-1; i >= 0; i--)
            {
                _loginAttempts.Remove(LoginAttempts[i]);
            }

            ProfileImage = null;

            return Result.Ok();
        }

        public Result UpdateSocials(UpdateSocialsDto dto)
        {
            if (dto == null)
            {
                return Result.Ok();
            }

            if (!string.IsNullOrEmpty(dto.Snapchat))
            {
                if (dto.Snapchat[0] == '@')
                {
                    dto.Snapchat = dto.Snapchat[1..];
                }

                if (dto.Snapchat.Length > 255)
                {
                    return Result.Failure("Your Snapchat username cannot be greater than 255 characters.");
                }

                Snapchat = dto.Snapchat;
            }
            else
            {
                Snapchat = null;
            }

            if (!string.IsNullOrEmpty(dto.Instagram))
            {
                if (dto.Instagram[0] == '@')
                {
                    dto.Instagram = dto.Instagram[1..];
                }

                if (dto.Instagram.Length > 255)
                {
                    return Result.Failure("Your Instagram username cannot be greater than 255 characters.");
                }

                Instagram = dto.Instagram;
            }
            else
            {
                Instagram = null;
            }

            if (!string.IsNullOrEmpty(dto.Twitter))
            {
                if (dto.Twitter[0] == '@')
                {
                    dto.Twitter = dto.Twitter[1..];
                }

                if (dto.Twitter.Length > 255)
                {
                    return Result.Failure("Your Twitter username cannot be greater than 255 characters.");
                }

                Twitter = dto.Twitter;
            }
            else
            {
                Twitter = null;
            }

            return Result.Ok();
        }

        public static Result<User> Create(RegisterUserDto dto, INormalizer normalizer, IPasswordHasher hasher)
        {
            var user = new User();
            return user.UpdateUsername(dto.Username, normalizer)
                .Bind(() => user.SetPassword(dto.Password, hasher))
                .Map(() => user);
        }
    }
}
