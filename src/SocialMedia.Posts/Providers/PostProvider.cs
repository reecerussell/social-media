using CSharpFunctionalExtensions;
using Dapper;
using MySql.Data.MySqlClient;
using SocialMedia.Core;
using SocialMedia.Core.Dtos;
using SocialMedia.Posts.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialMedia.Posts.Providers
{
    internal class PostProvider : IPostProvider
    {
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly IUser _user;

        public PostProvider(
            IConnectionStringProvider connectionStringProvider,
            IUser user)
        {
            _connectionStringProvider = connectionStringProvider;
            _user = user;
        }

        public async Task<Maybe<PostDto>> GetPostAsync(string id)
        {
            const string query = "CALL get_post(?postId, ?userId)";

            await using var connection = new MySqlConnection(
                await _connectionStringProvider.GetConnectionStringAsync());

            var args = new {postId = id, userId = _user.Id};
            var results = await connection.QueryMultipleAsync(query, args);
            var post = await results.ReadSingleOrDefaultAsync<PostDto>();
            if (post == null)
            {
                return null;
            }

            post.Comments = (IReadOnlyList<CommentDto>) (await results.ReadAsync<CommentDto>());

            return post;
        }

        public async Task<IReadOnlyList<FeedItemDto>> GetFeedAsync()
        {
            const string query = "CALL get_feed(?userId);";

            await using var connection = new MySqlConnection(
                await _connectionStringProvider.GetConnectionStringAsync());

            return (IReadOnlyList<FeedItemDto>) await connection.QueryAsync<FeedItemDto>(query, new {userId = _user.Id});
        }
    }
}
