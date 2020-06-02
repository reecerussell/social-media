using CSharpFunctionalExtensions;
using SocialMedia.Auth.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialMedia.Auth
{
    public interface IUserProvider
    {
        Task<Maybe<AccountDto>> GetUserAccountAsync();
        Task<IReadOnlyList<UserLoginAttemptDto>> GetUserLoginAttemptsAsync();
        Task<Maybe<ProfileDto>> GetProfileAsync(string username);
    }
}
