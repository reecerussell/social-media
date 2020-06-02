using CSharpFunctionalExtensions;
using Dapper;
using MySql.Data.MySqlClient;
using SocialMedia.Auth.Dtos;
using SocialMedia.Core;
using SocialMedia.Core.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialMedia.Auth.Providers
{
    internal class UserProvider : IUserProvider
    {
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly IUser _loggedInUser;
        private readonly INormalizer _normalizer;

        public UserProvider(
            IConnectionStringProvider connectionStringProvider,
            IUser loggedInUser,
            INormalizer normalizer)
        {
            _connectionStringProvider = connectionStringProvider;
            _loggedInUser = loggedInUser;
            _normalizer = normalizer;
        }

        public async Task<Maybe<AccountDto>> GetUserAccountAsync()
        {
            const string query = @"SELECT 
                                        id as `Id`,
                                        username as `Username`,
                                        bio as `Bio`,
                                        profile_image_id as `ProfileImageId`,
                                        snapchat as `Snapchat`,
                                        instagram as `Instagram`,
                                        twitter as `Twitter`
                                    FROM
                                        users
                                    WHERE id = ?userId;";

            await using var connection = new MySqlConnection(
                await _connectionStringProvider.GetConnectionStringAsync());

            return await connection.QuerySingleOrDefaultAsync<AccountDto>(query, new {userId = _loggedInUser.Id});
        }

        public async Task<IReadOnlyList<UserLoginAttemptDto>> GetUserLoginAttemptsAsync()
        {
            const string query = @"SELECT 
                                        remote_address AS `RemoteAddress`,
                                        successful AS `Successful`,
                                        message AS `Message`,
                                        `date` AS `Date`
                                    FROM
                                        user_login_attempts
                                    ORDER BY `date` DESC
                                    LIMIT 10;";

            await using var connection = new MySqlConnection(
                await _connectionStringProvider.GetConnectionStringAsync());

            return (IReadOnlyList<UserLoginAttemptDto>)await connection.QueryAsync<UserLoginAttemptDto>(
                query, 
                new {userId = _loggedInUser.Id});
        }

        public async Task<Maybe<ProfileDto>> GetProfileAsync(string username)
        {
            const string query = "CALL get_profile(?username, ?userId);";

            await using var connection = new MySqlConnection(
                await _connectionStringProvider.GetConnectionStringAsync());

            var args = new { username = _normalizer.Normalize(username), userId = _loggedInUser.Id };
            var results = await connection.QueryMultipleAsync(query, args);

            var profile = await results.ReadSingleOrDefaultAsync<ProfileDto>();
            if (profile == null)
            {
                return null;
            }

            profile.Posts = (IReadOnlyList<FeedItemDto>) await results.ReadAsync<FeedItemDto>();

            return profile;
        }
    }
}
