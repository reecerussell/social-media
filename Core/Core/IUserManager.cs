using Core.Dtos;
using CSharpFunctionalExtensions;
using System.Threading.Tasks;

namespace Core
{
    public interface IUserManager
    {
        Task<Result<string>> RegisterAsync(RegisterUserDto dto);
        Task<Result<AccessToken>> LoginAsync(IContext context, UserCredentialsDto dto);
        Task<Result> ChangePasswordAsync(IContext context, ChangePasswordDto dto);
        Task<Result> UpdateUsernameAsync(IContext context, UpdateUsernameDto dto);
        Task<Result> UpdateBioAsync(IContext context, UserBioDto dto);
        Task<Result> UpdateProfilePictureAsync(IContext context, UpdateMediaDto dto);
        Task<Result> DeleteAsync(IContext context, string userId);
        Task<Result> FollowAsync(IContext context, string userId);
        Task<Result> UnfollowAsync(IContext context, string userId);
    }
}
