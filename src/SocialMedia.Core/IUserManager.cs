using CSharpFunctionalExtensions;
using SocialMedia.Core.Dtos;
using System.Threading.Tasks;

namespace SocialMedia.Core
{
    public interface IUserManager
    {
        Task<Result> RegisterAsync(RegisterUserDto dto);
        Task<Result> LoginAsync(LoginUserDto dto);
        Task<Result> ChangePasswordAsync(ChangePasswordDto dto);
        Task<Result> ChangeProfilePictureAsync(UpdateMediaDto dto);
        Task<Result> UpdateAsync(UpdateUserDto dto);
        Task<Result> UpdateSocialsAsync(UpdateSocialsDto dto);
        Task<Result> DeleteAsync();
        Task<Result> FollowAsync(string userId);
        Task<Result> UnfollowAsync(string userId);
    }
}
