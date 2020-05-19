using Core.Dtos;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Core.Models
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

        private List<UserFollower> _followers;

        public IReadOnlyList<UserFollower> Followers
        {
            get => _lazyLoader.Load(this, ref _followers);
            set => _followers = (List<UserFollower>)value;
        }

        private readonly ILazyLoader _lazyLoader;

        private User(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
            Followers = new List<UserFollower>();
        }

        private User()
        {
            Id = Guid.NewGuid().ToString();
            ConcurrencyStamp = Guid.NewGuid().ToString();
        }

        internal Result UpdateUsername(string username, INormalizer normalizer)
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

        internal Result UpdateBio(string bio)
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

        internal Result UpdatePassword(ChangePasswordDto dto, IPasswordHasher hasher)
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

        internal Result CanLogin(string password, IPasswordHasher hasher)
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
                    return Result.Failure("Your account has been locked for too many failed login attempts.");
                }

                IncrementAccessFailedCount();
                return Result.Failure("Your account is currently locked.");
            }

            if (!hasher.Verify(PasswordHash, password))
            {
                IncrementAccessFailedCount();
                return Result.Failure("Your password is incorrect.");
            }

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

        internal void SetProfilePicture(Media media)
        {
            ProfileImageId = media.Id;
            ConcurrencyStamp = Guid.NewGuid().ToString();
        }

        internal Result AddFollower(User follower)
        {
            if (Followers.Any(x => x.FollowerId == follower.Id))
            {
                // The current user is already following this user.
                return Result.Failure("You're already following this user.");
            }

            return UserFollower.Create(Id, follower.Id)
                .Tap(uf => _followers.Add(uf));
        }

        internal Result RemoveFollower(User follower)
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

        internal static Result<User> Create(RegisterUserDto dto, INormalizer normalizer, IPasswordHasher hasher)
        {
            var user = new User();
            return user.UpdateUsername(dto.Username, normalizer)
                .Bind(() => user.SetPassword(dto.Password, hasher))
                .Map(() => user);
        }
    }
}
