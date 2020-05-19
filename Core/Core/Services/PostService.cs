using Core.Dtos;
using Core.Models;
using Core.Repositories;
using CSharpFunctionalExtensions;
using System.Threading.Tasks;

namespace Core.Services
{
    internal class PostService : IPostManager
    {
        private readonly PostRepository _repository;
        private readonly UserRepository _userRepository;
        private readonly MediaService _mediaService;

        public PostService(
            PostRepository repository,
            UserRepository userRepository,
            MediaService mediaService)
        {
            _repository = repository;
            _userRepository = userRepository;
            _mediaService = mediaService;
        }

        public async Task<Result<string>> CreateAsync(IContext context, CreatePostDto dto)
        {
            Media media = null;
            return await _userRepository.FindByIdAsync(context.GetUserId())
                .ToResult(CommonErrors.UserNotFound)
                .Tap(HandleMedia)
                .Bind(u => Post.Create(dto, u, media))
                .Tap(_repository.Add)
                .Tap(_ => _repository.SaveChangesAsync())
                .Map(p => p.Id);

            async Task<Result> HandleMedia(User user)
            {
                if (dto.File == null)
                {
                    return Result.Ok();
                }

                var info = new UpdateMediaDto
                {
                    ContentType = dto.ContentType,
                    File = dto.File,
                    Description = dto.Caption,
                };

                return await _mediaService.CreateAsync(context, info, user)
                    .Tap(m => media = m);
            }
        }

        public async Task<Result> DeleteAsync(IContext context, string id)
        {
            return await _repository.FindByIdAsync(id)
                .ToResult(CommonErrors.PostNotFound)
                .Tap(CanDelete)
                .Tap(_repository.Remove)
                .Tap(_ => _repository.SaveChangesAsync())
                .Tap(p => _mediaService.DeleteAsync(context, p.MediaId));

            Result CanDelete(Post post)
            {
                return post.UserId == context.GetUserId()
                    ? Result.Ok()
                    : Result.Failure("You are not authorized to delete this post.");
            }
        }

        public async Task<Result> AddCommentAsync(IContext context, CreateCommentDto dto)
        {
            User user = null;
            return await _userRepository.FindByIdAsync(context.GetUserId())
                .ToResult(CommonErrors.UserNotFound)
                .Tap(u => user = u)
                .Bind(_ => _repository.FindByIdAsync(dto.PostId)
                    .ToResult(CommonErrors.PostNotFound))
                .Tap(p => p.AddComment(user, dto))
                .Tap(_ => _repository.SaveChangesAsync());
        }

        public async Task<Result> DeleteCommentAsync(IContext context, string postId, string commentId)
        {
            return await _repository.FindByIdAsync(postId)
                .ToResult(CommonErrors.PostNotFound)
                .Tap(p => p.RemoveComment(context, commentId))
                .Tap(_ => _repository.SaveChangesAsync());
        }

        public async Task<Result> LikeAsync(IContext context, string id)
        {
            User user = null;
            return await _userRepository.FindByIdAsync(context.GetUserId())
                .ToResult("User could not be found.")
                .Tap(u => user = u)
                .Bind(_ => _repository.FindByIdAsync(id)
                    .ToResult("Post could not be found."))
                .Tap(p => p.Like(user))
                .Tap(_ => _repository.SaveChangesAsync());
        }

        public async Task<Result> UnlikeAsync(IContext context, string id)
        {
            User user = null;
            return await _userRepository.FindByIdAsync(context.GetUserId())
                .ToResult("User could not be found.")
                .Tap(u => user = u)
                .Bind(_ => _repository.FindByIdAsync(id)
                    .ToResult("Post could not be found."))
                .Tap(p => p.Unlike(user))
                .Tap(_ => _repository.SaveChangesAsync());
        }
    }
}
