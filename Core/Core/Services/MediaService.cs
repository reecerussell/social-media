using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Core.Dtos;
using Core.Models;
using Core.Repositories;
using CSharpFunctionalExtensions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Core.Services
{
    internal class MediaService
    {
        private readonly string _s3BucketName;
        private readonly IAmazonS3 _s3Client;
        private readonly MimeTypeRepository _mimeTypeRepository;
        private readonly MediaRepository _repository;

        public MediaService(
            MimeTypeRepository mimeTypeRepository,
            MediaRepository repository)
        {
            _s3BucketName = Environment.GetEnvironmentVariable("MEDIA_BUCKET_NAME");
            _s3Client = new AmazonS3Client();
            _mimeTypeRepository = mimeTypeRepository;
            _repository = repository;
        }

        public async Task<Result<Media>> CreateAsync(UpdateMediaDto dto, User user)
        {
            return await _mimeTypeRepository.FindByNameAsync(dto.ContentType)
                .ToResult($"'{dto.ContentType}' is an unsupported media type.")
                .Bind(mt => Media.Create(dto, user, mt))
                .Tap(m => _repository.Add(m))
                .Tap(m => UploadAsync(m, dto.File))
                .Tap(_ => _repository.SaveChangesAsync());
        }

        public async Task<Result<Media>> DeleteAsync(string id)
        {
            return await _repository.FindByIdAsync(id)
                .ToResult(CommonErrors.MediaNotFound)
                .Tap(m => _repository.Remove(m))
                .Tap(DeleteFileAsync)
                .Tap(_ => _repository.SaveChangesAsync());
        }

        private async Task<Result> UploadAsync(Media media, Stream file)
        {
            try
            {
                using var transferUtility = new TransferUtility(_s3Client);

                await using (file)
                {
                    await transferUtility.UploadAsync(file, _s3BucketName, media.Id);
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

        private async Task<Result> DeleteFileAsync(Media media)
        {
            try
            {
                await _s3Client.DeleteObjectAsync(new DeleteObjectRequest {Key = media.Id});

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
