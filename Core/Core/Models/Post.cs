using Core.Dtos;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Models
{
    public class Post
    {
        public string Id { get; private set; }
        public string UserId { get; private set; }
        public string MediaId { get; private set; }
        public DateTime DateCreated { get; private set; }
        public string Caption { get; private set; }

        private Media _media;
        public Media Media => _lazyLoader.Load(this, ref _media);

        private List<PostComment> _comments;
        public IReadOnlyList<PostComment> Comments => _lazyLoader.Load(this, ref _comments);

        private List<PostLike> _likes;
        public IReadOnlyList<PostLike> Likes => _lazyLoader.Load(this, ref _likes);

        private readonly ILazyLoader _lazyLoader;

        private Post(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        private Post(User user, Media media)
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            _media = media;
        }

        internal Result UpdateCaption(string caption)
        {
            if (string.IsNullOrEmpty(caption))
            {
                Caption = null;

                return Result.Ok();
            }

            if (caption.Length > 255)
            {
                return Result.Failure("Post caption cannot be greater than 255 characters.");
            }

            Caption = caption;

            return Result.Ok();
        }

        internal Result AddComment(User user, CreateCommentDto dto)
        {
            _ = Comments.Count;

            PostComment replyTo = null;
            if (!string.IsNullOrEmpty(dto.ReplyToId))
            {
                replyTo = Comments.SingleOrDefault(x => x.Id == dto.ReplyToId);
            }

            return PostComment.Create(dto.Comment, this, user, replyTo)
                .Tap(_comments.Add);
        }

        internal Result RemoveComment(IContext context, string id)
        {
            var comment = Comments.SingleOrDefault(x => x.Id == id);
            if (comment == null)
            {
                return Result.Failure("Comment could not be found.");
            }

            if (context.GetUserId() != comment.Id)
            {
                return Result.Failure("You are not authorized to delete this comment.");
            }

            _comments.Remove(comment);

            return Result.Ok();
        }

        internal Result Like(User user)
        {
            var like = Likes.SingleOrDefault(x => x.UserId == user.Id);
            if (like != null)
            {
                // The given user has already liked this post.
                return Result.Failure("You've already liked this post!");
            }

            return PostLike.Create(this, user)
                .Tap(_likes.Add);
        }

        internal Result Unlike(User user)
        {
            var like = Likes.SingleOrDefault(x => x.UserId == user.Id);
            if (like == null)
            {
                // The given user hasn't liked this post.
                return Result.Failure("You haven't liked this post.");
            }

            _likes.Remove(like);

            return Result.Ok();
        }

        internal static Result<Post> Create(CreatePostDto dto, User user, Media media = null)
        {
            var post = new Post(user, media);
            return post.UpdateCaption(dto.Caption)
                .Map(() => post);
        }
    }
}
