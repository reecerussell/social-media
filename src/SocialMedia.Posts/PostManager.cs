using CSharpFunctionalExtensions;
using SocialMedia.Core;
using SocialMedia.Core.Dtos;
using SocialMedia.Domain.Models;
using SocialMedia.Posts.Repository;
using System.Threading.Tasks;

namespace SocialMedia.Posts
{
    internal class PostManager : IPostManager
    {
        private readonly PostRepository _repository;
        private readonly IMediaService _mediaService;
        private readonly IUser _user;

        public PostManager(
            PostRepository repository,
            IMediaService mediaService,
            IUser user)
        {
            _repository = repository;
            _mediaService = mediaService;
            _user = user;
        }

        public async Task<Result<string>> CreateAsync(CreatePostDto dto)
        {
            string mediaId = null;
            return await HandleMedia()
                .Bind(() => Post.Create(dto, _user, mediaId))
                .Tap(_repository.Add)
                .Tap(_ => _repository.SaveChangesAsync())
                .Map(p => p.Id);

            async Task<Result> HandleMedia()
            {
                if (dto.File == null)
                {
                    return Result.Ok();
                }

                var info = new UpdateMediaDto
                {
                    File = dto.File,
                    Description = dto.Caption,
                };

                return await _mediaService.CreateAsync(info)
                    .Tap(id => mediaId = id);
            }
        }

        public async Task<Result> DeleteAsync(string postId)
        {
            return await _repository.FindByIdAsync(postId)
                .ToResult(CommonErrors.PostNotFound)
                .Tap(p => p.CanDelete(_user))
                .Tap(_repository.Remove)
                .Tap(_ => _repository.SaveChangesAsync())
                .Tap(DeleteMedia);

            async Task<Result> DeleteMedia(Post post)
            {
                if (!string.IsNullOrEmpty(post.MediaId))
                {
                    return Result.Ok();
                }

                return await _mediaService.DeleteAsync(post.MediaId);
            }
        }

        public async Task<Result> AddCommentAsync(CreateCommentDto dto)
        {
            return await _repository.FindByIdWithCommentsAsync(dto.PostId)
                .ToResult(CommonErrors.PostNotFound)
                .Tap(p => p.AddComment(_user, dto))
                .Tap(_ => _repository.SaveChangesAsync());
        }

        public async Task<Result> DeleteCommentAsync(string postId, string commentId)
        {
            return await _repository.FindByIdWithCommentsAsync(postId)
                .ToResult(CommonErrors.PostNotFound)
                .Tap(p => p.RemoveComment(_user, commentId))
                .Tap(_ => _repository.SaveChangesAsync());
        }

        public async Task<Result> LikeAsync(string postId)
        {
            return await _repository.FindByIdWithLikesAsync(postId)
                .ToResult(CommonErrors.PostNotFound)
                .Tap(p => p.Like(_user))
                .Tap(_ => _repository.SaveChangesAsync());
        }

        public async Task<Result> UnlikeAsync(string postId)
        {
            return await _repository.FindByIdWithLikesAsync(postId)
                .ToResult(CommonErrors.PostNotFound)
                .Tap(p => p.Unlike(_user))
                .Tap(_ => _repository.SaveChangesAsync());
        }
    }
}
