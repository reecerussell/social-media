using CSharpFunctionalExtensions;
using SocialMedia.Core;
using SocialMedia.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialMedia.Domain.Models
{
    public class Post
    {
        public string Id { get; private set; }
        public string UserId { get; private set; }
        public string MediaId { get; private set; }
        public DateTime DateCreated { get; private set; }
        public string Caption { get; private set; }

        public Media Media { get; private set; }

        private List<PostComment> _comments;
        public IReadOnlyList<PostComment> Comments
        {
            get => _comments;
            set => _comments = (List<PostComment>) value;
        }

        private List<PostLike> _likes;
        public IReadOnlyList<PostLike> Likes
        {
            get => _likes;
            set => _likes = (List<PostLike>) value;
        }

        private Post()
        {
        }

        private Post(IUser user, string mediaId)
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            UserId = user.Id;
            MediaId = mediaId;
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
        
        public Result AddComment(IUser user, CreateCommentDto dto)
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

        public Result RemoveComment(IUser user, string id)
        {
            var comment = Comments.SingleOrDefault(x => x.Id == id);
            if (comment == null)
            {
                return Result.Failure("Comment could not be found.");
            }

            if (user.Id != comment.UserId)
            {
                return Result.Failure("You are not authorized to delete this comment.");
            }

            comment.DeleteReplies();
            _comments.Remove(comment);

            return Result.Ok();
        }

        public Result Like(IUser user)
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

        public Result Unlike(IUser user)
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

        public Result CanDelete(IUser user)
        {
            if (user.Id != UserId)
            {
                return Result.Failure("You're not authorised to delete this post.");
            }

            return Result.Ok();
        }

        public static Result<Post> Create(CreatePostDto dto, IUser user, string mediaId = null)
        {
            var post = new Post(user, mediaId);
            return post.UpdateCaption(dto.Caption)
                .Map(() => post);
        }
    }
}
