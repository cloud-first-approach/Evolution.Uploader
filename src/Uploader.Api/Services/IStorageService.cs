using Uploader.Api.Models;

namespace Uploader.Api.Services
{
    public interface IStorageService
    {
        Task SaveVideo(UploadVideoRequestModel videoRequestModel);
        public Task<string> UploadVideoToS3BucketAsync(UploadVideoRequestModel requestDto);
    }
}