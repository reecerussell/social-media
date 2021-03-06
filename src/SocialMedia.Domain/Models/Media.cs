﻿using CSharpFunctionalExtensions;
using SocialMedia.Core.Dtos;
using System;
using SocialMedia.Core;

namespace SocialMedia.Domain.Models
{
    public class Media
    {
        public string Id { get; private set; }
        public string UserId { get; private set; }
        public string MimeTypeId { get; private set; }
        public string Description { get; private set; }

        public MimeType MimeType { get; private set; }

        private Media()
        {
        }

        private Media(IUser user, MimeType mimeType)
        {
            Id = Guid.NewGuid().ToString();
            UserId = user.Id;
            MimeTypeId = mimeType.Id;
        }

        internal Result UpdateDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                Description = null;

                return Result.Ok();
            }

            if (description.Length > 255)
            {
                return Result.Failure("Media description cannot be greater than 255 characters.");
            }

            Description = description;

            return Result.Ok();
        }

        public static Result<Media> Create(UpdateMediaDto dto, IUser user, MimeType mimeType)
        {
            var media = new Media(user, mimeType);
            return media.UpdateDescription(dto.Description)
                .Map(() => media);
        }
    }
}
