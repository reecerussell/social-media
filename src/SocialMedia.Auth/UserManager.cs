using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SocialMedia.Auth.Repositories;
using SocialMedia.Core;
using SocialMedia.Core.Dtos;
using SocialMedia.Domain.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SocialMedia.Auth
{
    internal class UserManager : IUserManager
    {
        private readonly UserRepository _repository;
        private readonly INormalizer _normalizer;
        private readonly IPasswordHasher _hasher;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<UserManager> _logger;
        private readonly IUser _loggedInUser;
        private readonly IMediaService _mediaService;

        public UserManager(
            UserRepository repository,
            INormalizer normalizer,
            IPasswordHasher hasher,
            IHttpContextAccessor contextAccessor,
            ILogger<UserManager> logger,
            IUser user,
            IMediaService mediaService)
        {
            _repository = repository;
            _normalizer = normalizer;
            _hasher = hasher;
            _contextAccessor = contextAccessor;
            _logger = logger;
            _loggedInUser = user;
            _mediaService = mediaService;
        }

        public async Task<Result> RegisterAsync(RegisterUserDto dto)
        {
            return await User.Create(dto, _normalizer, _hasher)
                .Tap(_repository.Add)
                .Tap(u => _repository.SaveChangesAsync())
                .Tap(u => SignInAsync(u, false))
                .Tap(LogRegistration);

            void LogRegistration(User user)
            {
                var eventId = new EventId(1, user.Id);
                var remoteAddress = _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                var message = $"User registered with username '{user.Username}' from {remoteAddress}!";
                _logger.LogInformation(eventId, message);
            }
        }

        public async Task<Result> LoginAsync(LoginUserDto dto)
        {
            var (success, _, user, error) = await _repository.FindByUsernameAsync(dto.Username)
                .ToResult("No account matches those details.")
                .Tap(u => u.CanLogin(dto.Password, _hasher, _contextAccessor.HttpContext));

            await _repository.SaveChangesAsync();

            if (!success)
            {
                return Result.Failure(error);
            }

            await SignInAsync(user, dto.RememberMe);

            return Result.Ok();
        }

        public async Task<Result> ChangePasswordAsync(ChangePasswordDto dto)
        {
            return await _repository.FindByIdAsync(_loggedInUser.Id)
                .ToResult(CommonErrors.UserNotFound)
                .Tap(u => u.UpdatePassword(dto, _hasher))
                .Bind(_ => _repository.SaveChangesAsync());
        }

        public async Task<Result> ChangeProfilePictureAsync(UpdateMediaDto dto)
        {
            return await _repository.FindByIdAsync(_loggedInUser.Id)
                .ToResult(CommonErrors.UserNotFound)
                .Tap(DeleteMedia)
                .Tap(UploadMedia)
                .Tap(_ => _repository.SaveChangesAsync());

            async Task<Result> DeleteMedia(User user)
            {
                if (string.IsNullOrEmpty(user.ProfileImageId))
                {
                    return Result.Ok();
                }

                return await _mediaService.DeleteAsync(user.ProfileImageId);
            }

            async Task<Result> UploadMedia(User user)
            {
                if (dto.File == null)
                {
                    return Result.Ok();
                }

                return await _mediaService.CreateAsync(dto)
                    .Tap(user.SetProfilePicture);
            }
        }

        public async Task<Result> UpdateAsync(UpdateUserDto dto)
        {
            return await _repository.FindByIdAsync(_loggedInUser.Id)
                .ToResult(CommonErrors.UserNotFound)
                .Ensure(async _ => !await _repository.ExistsWithUsernameAsync(dto.Username, _loggedInUser.Id),
                    $"The username '{dto.Username}' has already been taken!")
                .Tap(u => u.Update(dto, _normalizer))
                .Bind(_ => _repository.SaveChangesAsync());
        }

        public async Task<Result> UpdateSocialsAsync(UpdateSocialsDto dto)
        {
            return await _repository.FindByIdAsync(_loggedInUser.Id)
                .ToResult(CommonErrors.UserNotFound)
                .Tap(u => u.UpdateSocials(dto))
                .Bind(_ => _repository.SaveChangesAsync());
        }

        public async Task<Result> DeleteAsync()
        {
            return await _repository.FindByIdWithRelatedDataAsync(_loggedInUser.Id)
                .ToResult(CommonErrors.UserNotFound)
                .Tap(u => u.Delete(_loggedInUser))
                .Tap(_repository.Remove)
                .Tap(u => _repository.SaveChangesAsync())
                .Tap(u => _mediaService.DeleteMediaForUserAsync(u.Id))
                .Tap(LogDeletion);

            void LogDeletion(User user)
            {
                var remoteAddress = _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                var message = $"User '{user.Id}' deleted their account from {remoteAddress}!";
                _logger.LogInformation(message);
            }
        }

        public async Task<Result> FollowAsync(string userId)
        {
            User targetUser = null;
            return await _repository.FindByIdWithFollowersAsync(userId)
                .ToResult(CommonErrors.UserNotFound)
                .Tap(u => targetUser = u)
                .Bind(_ => _repository.FindByIdAsync(_loggedInUser.Id)
                    .ToResult(CommonErrors.UserNotFound))
                .Tap(u => targetUser.AddFollower(u))
                .Bind(_ => _repository.SaveChangesAsync());
        }

        public async Task<Result> UnfollowAsync(string userId)
        {
            User targetUser = null;
            return await _repository.FindByIdWithFollowersAsync(userId)
                .ToResult(CommonErrors.UserNotFound)
                .Tap(u => targetUser = u)
                .Bind(_ => _repository.FindByIdAsync(_loggedInUser.Id)
                    .ToResult(CommonErrors.UserNotFound))
                .Tap(u => targetUser.RemoveFollower(u))
                .Bind(_ => _repository.SaveChangesAsync());
        }

        private Task SignInAsync(User user, bool isPersistent)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.UserIdClaim, user.Id));
            identity.AddClaim(new Claim(ClaimTypes.UsernameClaim, user.Username));
            var principal = new ClaimsPrincipal(identity);

            return _contextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties {IsPersistent = isPersistent});
        }
    }
}
