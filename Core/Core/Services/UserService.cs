using Core.Dtos;
using Core.Models;
using Core.Repositories;
using CSharpFunctionalExtensions;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Core.Services
{
    internal class UserService : IUserManager
    {
        private readonly IPasswordHasher _hasher;
        private readonly INormalizer _normalizer;
        private readonly ISecretProvider _secretProvider;
        private readonly UserRepository _repository;
        private readonly MediaService _mediaService;

        public UserService(
            IPasswordHasher hasher,
            INormalizer normalizer,
            ISecretProvider secretProvider,
            UserRepository repository,
            MediaService mediaService)
        {
            _hasher = hasher;
            _normalizer = normalizer;
            _secretProvider = secretProvider;
            _repository = repository;
            _mediaService = mediaService;
        }

        public async Task<Result<string>> RegisterAsync(RegisterUserDto dto)
        {
            return await User.Create(dto, _normalizer, _hasher)
                .Ensure(async _ => !await _repository.ExistsWithUsernameAsync(dto.Username), 
                    $"The username '{dto.Username}' has already been taken.")
                .Tap(u => _repository.Add(u))
                .Tap(u => _repository.SaveChangesAsync())
                .Map(u => u.Id);
        }

        public async Task<Result<AccessToken>> LoginAsync(IContext context, UserCredentialsDto dto)
        {
            return await _repository.FindByUsernameAsync(dto.Username)
                .ToResult($"No account with the username '{dto.Username}' exists.")
                .Tap(u => u.CanLogin(dto.Password, _hasher))
                .Bind(GenerateAccessToken)
                .Tap(ac => _repository.SaveChangesAsync());

            async Task<Result<AccessToken>> GenerateAccessToken(User user)
            {
                var secretName = context.GetValue<string>("AUTH_SECRET_NAME");
                if (string.IsNullOrEmpty(secretName)) throw new ArgumentNullException(nameof(secretName), "the 'AUTH_SECRET_NAME' has not been set");

                var crypto = await _secretProvider.GetRsa(secretName, "private");
                var signingCredentials = new SigningCredentials(new RsaSecurityKey(crypto), SecurityAlgorithms.RsaSha256);
                var jwtHeader = new JwtHeader(signingCredentials);

                var nowUtc = DateTime.UtcNow;
                var expires = nowUtc.AddSeconds(3600);
                var centuryBegin = new DateTime(1970, 1, 1);
                var exp = (long)new TimeSpan(expires.Ticks - centuryBegin.Ticks).TotalSeconds;
                var now = (long)new TimeSpan(nowUtc.Ticks - centuryBegin.Ticks).TotalSeconds;
                var issuer = "SocialMedia";

                var payload = new JwtPayload
                {
                    {"sub", user.Id},
                    {ClaimTypes.NameIdentifier, user.Id},
                    { "iss", issuer},
                    {"iat", now},
                    {"nbf", now},
                    {"exp", exp},
                    {"jti", Guid.NewGuid().ToString("N")}
                };

                var jwt = new JwtSecurityToken(jwtHeader, payload);
                var token = new JwtSecurityTokenHandler().WriteToken(jwt);

                return Result.Ok(new AccessToken
                {
                    Token = token,
                    Expires = exp,
                });
            }
        }

        public async Task<Result> ChangePasswordAsync(IContext context, ChangePasswordDto dto)
        {
            return await _repository.FindByIdAsync(context.GetUserId())
                .ToResult(CommonErrors.UserNotFound)
                .Tap(u => u.UpdatePassword(dto, _hasher))
                .Bind(_ => _repository.SaveChangesAsync());
        }

        public async Task<Result> UpdateUsernameAsync(IContext context, UpdateUsernameDto dto)
        {
            return await _repository.FindByIdAsync(context.GetUserId())
                .ToResult(CommonErrors.UserNotFound)
                .Ensure(async u => !await _repository.ExistsWithUsernameAsync(dto.Username, u.Id), 
                    $"The username '{dto.Username}' has already been taken.")
                .Tap(u => u.UpdateUsername(dto.Username, _normalizer))
                .Bind(_ => _repository.SaveChangesAsync());
        }

        public async Task<Result> UpdateBioAsync(IContext context, UserBioDto dto)
        {
            return await _repository.FindByIdAsync(context.GetUserId())
                .ToResult(CommonErrors.UserNotFound)
                .Tap(u => u.UpdateBio(dto.Bio))
                .Bind(_ => _repository.SaveChangesAsync());
        }

        public async Task<Result> UpdateProfilePictureAsync(IContext context, UpdateMediaDto dto)
        {
            return await _repository.FindByIdAsync(context.GetUserId())
                .ToResult(CommonErrors.UserNotFound)
                .Tap(RemoveMedia)
                .Tap(CreateMedia)
                .Tap(_ => _repository.SaveChangesAsync());

            async Task<Result> RemoveMedia(User user)
            {
                if (user.ProfileImageId == null)
                {
                    return Result.Ok();
                }

                return await _mediaService.DeleteAsync(context, user.ProfileImageId);
            }

            async Task<Result> CreateMedia(User user)
            {
                return await _mediaService.CreateAsync(context, dto, user)
                    .Tap(user.SetProfilePicture);
            }
        }

        public async Task<Result> DeleteAsync(string userId)
        {
            return await _repository.FindByIdAsync(userId)
                .ToResult(CommonErrors.UserNotFound)
                .Tap(u => _repository.Remove(u))
                .Bind(_ => _repository.SaveChangesAsync());
        }

        public async Task<Result> FollowAsync(IContext context, string userId)
        {
            User currentUser = null;
            return await _repository.FindByIdAsync(context.GetUserId())
                .ToResult(CommonErrors.UserNotFound)
                .Tap(u => currentUser = u)
                .Bind(_ => _repository.FindByIdAsync(userId)
                    .ToResult(CommonErrors.UserNotFound))
                .Tap(u => u.AddFollower(currentUser))
                .Tap(_ => _repository.SaveChangesAsync());
        }

        public async Task<Result> UnfollowAsync(IContext context, string userId)
        {
            User currentUser = null;
            return await _repository.FindByIdAsync(context.GetUserId())
                .ToResult(CommonErrors.UserNotFound)
                .Tap(u => currentUser = u)
                .Bind(_ => _repository.FindByIdAsync(userId)
                    .ToResult(CommonErrors.UserNotFound))
                .Tap(u => u.RemoveFollower(currentUser))
                .Tap(_ => _repository.SaveChangesAsync());
        }
    }
}
