using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Core.Dtos;
using Core.Models;
using Core.Repositories;
using CSharpFunctionalExtensions;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Core.Services
{
    internal class MediaService : IMediaDownloader
    {
        private readonly IAmazonS3 _s3Client;
        private readonly MimeTypeRepository _mimeTypeRepository;
        private readonly MediaRepository _repository;

        public MediaService(
            IAmazonS3 s3Client,
            MimeTypeRepository mimeTypeRepository,
            MediaRepository repository)
        {
            _s3Client = s3Client;
            _mimeTypeRepository = mimeTypeRepository;
            _repository = repository;
        }

        public async Task<Result<Media>> CreateAsync(IContext context, UpdateMediaDto dto, User user)
        {
            return await _mimeTypeRepository.FindByNameAsync(dto.ContentType)
                .ToResult($"'{dto.ContentType}' is an unsupported media type.")
                .Bind(mt => Media.Create(dto, user, mt))
                .Tap(m => _repository.Add(m))
                .Tap(m => UploadAsync(context, m, dto.File))
                .Tap(_ => _repository.SaveChangesAsync());
        }

        public async Task<Result<Media>> DeleteAsync(IContext context, string id)
        {
            return await _repository.FindByIdAsync(id)
                .ToResult(CommonErrors.MediaNotFound)
                .Tap(_repository.Remove)
                .Tap(m => DeleteFileAsync(context, m))
                .Tap(_ => _repository.SaveChangesAsync());
        }

        public async Task<Result<DownloadMediaDto>> DownloadAsync(IContext context, string id)
        {
            DownloadMediaDto dto = null;
            return await _repository.FindByIdAsNoTrackingAsync(id)
                .ToResult(CommonErrors.MediaNotFound)
                .Tap(m => dto = new DownloadMediaDto
                {
                    ContentType = m.MimeType.Name,
                    Description = m.Description,
                })
                .Bind(_ => DownloadFromS3Async(context, id))
                .Tap(data => dto.Data = Convert.ToBase64String(data))
                .Map(_ => dto);
        }

        private async Task<Result> UploadAsync(IContext context, Media media, Stream file)
        {
            var bucketName = context.GetValue<string>("MEDIA_BUCKET_NAME");
            if (string.IsNullOrEmpty(bucketName)) throw new ArgumentNullException(nameof(bucketName), "the 'MEDIA_BUCKET_NAME' variable has no value");

            try
            {
                using var transferUtility = new TransferUtility(_s3Client);

                await using (file)
                {
                    await transferUtility.UploadAsync(file, bucketName, media.Id);
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

        private async Task<Result<byte[]>> DownloadFromS3Async(IContext context, string id)
        {
            var bucketName = context.GetValue<string>("MEDIA_BUCKET_NAME");
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

                var buffer = new byte[res.ResponseStream.Length];
                await res.ResponseStream.ReadAsync(buffer, 0, buffer.Length);

                return Result.Ok(buffer);
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

        private async Task<Result> DeleteFileAsync(IContext context, Media media)
        {
            var bucketName = context.GetValue<string>("MEDIA_BUCKET_NAME");
            if (string.IsNullOrEmpty(bucketName)) throw new ArgumentNullException(nameof(bucketName), "the 'MEDIA_BUCKET_NAME' variable has no value");

            try
            {
                await _s3Client.DeleteObjectAsync(new DeleteObjectRequest {Key = media.Id, BucketName = bucketName});

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
