using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using SocialMedia.Core;

namespace SocialMedia.Domain.Models
{
    public class PostComment
    {
        public string Id { get; private set; }
        public string PostId { get; private set; }
        public string UserId { get; private set; }
        public string ReplyToId { get; private set; }
        public DateTime DateCreated { get; private set; }
        public string Text { get; private set; }

        private List<PostComment> _replies;

        public IReadOnlyList<PostComment> Replies
        {
            get => _replies;
            set => _replies = (List<PostComment>) value;
        }

        private PostComment()
        {
        }

        private PostComment(string postId, string userId, string replyToId = null)
        {
            if (string.IsNullOrEmpty(postId))
            {
                throw new ArgumentNullException(nameof(postId));
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            Id = Guid.NewGuid().ToString();
            PostId = postId;
            UserId = userId;
            ReplyToId = replyToId;
            DateCreated = DateTime.Now;
        }

        internal Result UpdateText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Text = null;

                return Result.Ok();
            }

            if (text.Length > 255)
            {
                return Result.Failure("A commend cannot be greater than 255 characters.");
            }

            Text = text;

            return Result.Ok();
        }

        public void DeleteReplies()
        {
            _replies.Clear();
        }

        public static Result<PostComment> Create(string text, Post post, IUser user,
            PostComment replyTo = null)
        {
            string replyToId = null;
            if (replyTo != null)
            {
                // If the replyTo comment is replying to another comment, set the replyToId
                // to the target of the replyTo comment.
                replyToId = !string.IsNullOrEmpty(replyTo.ReplyToId) ?
                    replyTo.ReplyToId :
                    replyTo.Id;
            }

            var comment = new PostComment(post.Id, user.Id, replyToId);
            return comment.UpdateText(text)
                .Map(() => comment);
        }
    }
}
