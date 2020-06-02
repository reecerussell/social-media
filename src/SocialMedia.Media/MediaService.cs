using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SocialMedia.Core;
using SocialMedia.Core.Dtos;
using SocialMedia.Media.Repositories;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace SocialMedia.Media
{
    internal class MediaService : IMediaService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly MimeTypeRepository _mimeTypeRepository;
        private readonly MediaRepository _repository;
        private readonly IUser _user;
        private readonly IConfiguration _configuration;

        public MediaService(
            IAmazonS3 s3Client,
            MimeTypeRepository mimeTypeRepository,
            MediaRepository repository,
            IUser user,
            IConfiguration configuration)
        {
            _s3Client = s3Client;
            _mimeTypeRepository = mimeTypeRepository;
            _repository = repository;
            _user = user;
            _configuration = configuration;
        }

        public async Task<Result<string>> CreateAsync(UpdateMediaDto dto)
        {
            return await _mimeTypeRepository.FindByNameAsync(dto.File.ContentType)
                .ToResult($"'{dto.File.ContentType}' is an unsupported media type.")
                .Bind(mt => Domain.Models.Media.Create(dto, _user, mt))
                .Tap(m => _repository.Add(m))
                .Tap(m => UploadAsync(m, dto.File))
                .Tap(_ => _repository.SaveChangesAsync())
                .Map(m => m.Id);
        }

        public async Task<Result> DeleteAsync(string id)
        {
            return await _repository.FindByIdAsync(id)
                .ToResult(CommonErrors.MediaNotFound)
                .Tap(_repository.Remove)
                .Tap(DeleteFileAsync)
                .Tap(_ => _repository.SaveChangesAsync());
        }

        public async Task<Result> DeleteMediaForUserAsync(string userId)
        {
            var media = await _repository.FindByUserIdAsync(userId);
            var count = media.Count;

            for (var i = 0; i < count; i++)
            {
                _repository.Remove(media[i]);
                await DeleteFileAsync(media[i]);
            }

            return Result.Ok();
        }

        public async Task<Result<(byte[] Data, string ContentType)>> DownloadAsync(string id)
        {
            string contentType = null;
            return await _repository.FindByIdAsNoTrackingAsync(id)
                .ToResult(CommonErrors.MediaNotFound)
                .Tap(m => contentType = m.MimeType.Name)
                .Bind(_ => DownloadFromS3Async(id))
                .Map(data => (data, contentType));
        }

        private async Task<Result> UploadAsync(Domain.Models.Media media, IFormFile file)
        {
            var bucketName = _configuration[Constants.MediaBucketName];
            if (string.IsNullOrEmpty(bucketName)) throw new ArgumentNullException(nameof(bucketName), "the 'MEDIA_BUCKET_NAME' variable has no value");

            try
            {
                using var transferUtility = new TransferUtility(_s3Client);

                await using (var stream = file.OpenReadStream())
                {
                    await transferUtility.UploadAsync(stream, bucketName, media.Id);
                }

                return Result.Ok();
            }
            catch (AmazonS3Exception e)
            {
                return Result.Failure("An error occured on the server while uploading media content: " + e.Message);
            }
            catch (Exception e)
            {
                return Result.Failure("An unknown error occured on the server while uploading media content: " + e.Message);
            }
        }

        private async Task<Result<byte[]>> DownloadFromS3Async(string id)
        {
            var bucketName = _configuration[Constants.MediaBucketName];
            if (string.IsNullOrEmpty(bucketName)) throw new ArgumentNullException(nameof(bucketName), "the 'MEDIA_BUCKET_NAME' variable has no value");

            try
            {
                var res = await _s3Client.GetObjectAsync(bucketName, id);
                if (res.HttpStatusCode != HttpStatusCode.OK &&
                    res.HttpStatusCode != HttpStatusCode.NotModified)
                {
                    // Just return 404.
                    return Result.Failure<byte[]>(CommonErrors.MediaNotFound);
                }

                await using var ms = new MemoryStream();
                await res.ResponseStream.CopyToAsync(ms);

                return Result.Ok(ms.ToArray());
            }
            catch (AmazonS3Exception e)
            {
                throw new Exception("An error occured on the server while downloading media content: " + e.Message, e);
            }
            catch (Exception e)
            {
                throw new Exception("An unknown error occured on the server while downloading media content: " + e.Message, e);
            }
        }

        private async Task<Result> DeleteFileAsync(Domain.Models.Media media)
        {
            var bucketName = _configuration[Constants.MediaBucketName];
            if (string.IsNullOrEmpty(bucketName)) throw new ArgumentNullException(nameof(bucketName), "the 'MEDIA_BUCKET_NAME' variable has no value");

            try
            {
                await _s3Client.DeleteObjectAsync(new DeleteObjectRequest { Key = media.Id, BucketName = bucketName });

                return Result.Ok();
            }
            catch (AmazonS3Exception e)
            {
                return Result.Failure("An error occured on the server while uploading media content: " + e.Message);
            }
            catch (Exception e)
            {
                return Result.Failure("An unknown error occured on the server while uploading media content: " + e.Message);
            }
        }
    }
}
