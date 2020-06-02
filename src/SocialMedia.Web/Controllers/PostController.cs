using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Core;
using SocialMedia.Core.Dtos;
using SocialMedia.Posts;
using SocialMedia.Web.ViewModels;
using System.Threading.Tasks;

namespace SocialMedia.Web.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostManager _postManager;
        private readonly IPostProvider _postProvider;

        private const string CreateErrorKey = "postCreate:error";

        public PostController(
            IPostManager postManager,
            IPostProvider postProvider)
        {
            _postManager = postManager;
            _postProvider = postProvider;
        }

        [Route("Post/{id}")]
        public async Task<IActionResult> Index(string id)
        {
            var postOrNothing = await _postProvider.GetPostAsync(id);
            if (postOrNothing.HasNoValue)
            {
                return NotFound();
            }

            return View(postOrNothing.Value);
        }

        public IActionResult Create()
        {
            var viewModel = new CreatePostViewModel();

            if (TempData[CreateErrorKey] is string error)
            {
                viewModel.Error = error;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePostViewModel model)
        {
            var (success, _, id, error) = await _postManager.CreateAsync(model.Post);
            if (!success)
            {
                if (CommonErrors.IsNotFound(error))
                {
                    return NotFound();
                }

                TempData[CreateErrorKey] = error;

                return RedirectToAction(nameof(Create));
            }

            return RedirectToAction(nameof(Index), new {id});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var (success, _, error) = await _postManager.DeleteAsync(id);
            if (!success)
            {
                if (CommonErrors.IsNotFound(error))
                {
                    return NotFound();
                }

                return BadRequest(error);
            }

            return Redirect("/");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(CreateCommentDto dto)
        {
            var (success, _, error) = await _postManager.AddCommentAsync(dto);
            if (!success)
            {
                if (CommonErrors.IsNotFound(error))
                {
                    return NotFound();
                }

                TempData["commentError"] = error;
            }

            return RedirectToAction(nameof(Index), new {id = dto.PostId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(string postId, string commentId)
        {
            var (success, _, error) = await _postManager.DeleteCommentAsync(postId, commentId);
            if (!success)
            {
                if (CommonErrors.IsNotFound(error))
                {
                    return NotFound();
                }

                return BadRequest(error);
            }

            return RedirectToAction(nameof(Index), new { id = postId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Like(string postId)
        {
            var (success, _, error) = await _postManager.LikeAsync(postId);
            if (!success)
            {
                if (CommonErrors.IsNotFound(error))
                {
                    return NotFound();
                }

                return BadRequest(error);
            }

            return RedirectToAction(nameof(Index), new { id = postId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlike(string postId)
        {
            var (success, _, error) = await _postManager.UnlikeAsync(postId);
            if (!success)
            {
                if (CommonErrors.IsNotFound(error))
                {
                    return NotFound();
                }

                return BadRequest(error);
            }

            return RedirectToAction(nameof(Index), new { id = postId });
        }
    }
}
