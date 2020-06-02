using Microsoft.AspNetCore.Http;
using SocialMedia.Auth.Dtos;
using SocialMedia.Core;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace SocialMedia.Auth
{
    internal class CurrentUser : IUser
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConnectionStringProvider _connectionStringProvider;

        private LoggedInUserDto _userInfo;

        public CurrentUser(
            IHttpContextAccessor contextAccessor,
            IConnectionStringProvider connectionStringProvider)
        {
            _contextAccessor = contextAccessor;
            _connectionStringProvider = connectionStringProvider;
        }

        public string Id => GetUserId();

        public string Username
        {
            get
            {
                if (_userInfo == null)
                {
                    _userInfo = GetLoggedInUserInfoAsync().Result;
                }

                return _userInfo.Username;
            }
        }

        public int FollowerCount
        {
            get
            {
                if (_userInfo == null)
                {
                    _userInfo = GetLoggedInUserInfoAsync().Result;
                }

                return _userInfo.FollowerCount;
            }
        }

        public string ProfileImageId
        {
            get
            {
                if (_userInfo == null)
                {
                    _userInfo = GetLoggedInUserInfoAsync().Result;
                }

                return _userInfo.ProfileImageId;
            }
        }

        private string GetUserId()
        {
            var context = _contextAccessor.HttpContext;
            var claims = context.User.Identities.SelectMany(x => x.Claims);
            var idClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.UserIdClaim);

            return idClaim?.Value;
        }

        private async Task<LoggedInUserDto> GetLoggedInUserInfoAsync()
        {
            const string query = @"SELECT 
                                        u.username AS `Username`,
                                        u.profile_image_id AS `ProfileImageId`,
                                        (SELECT 
                                                COUNT(*)
                                            FROM
                                                user_followers
                                            WHERE
                                                user_id = u.id) AS `FollowerCount`
                                    FROM
                                        users AS u
                                    WHERE u.id = ?userId;";

            await using var connection = new MySqlConnection(
                await _connectionStringProvider.GetConnectionStringAsync());

            return await connection.QuerySingleOrDefaultAsync<LoggedInUserDto>(query, new { userId = Id });
        }
    }
}
