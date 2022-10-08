using Uploader.Api.Models;
using Uploader.Api.Services.Models;

namespace Uploader.Api.Services
{
    public interface IStorageService
    {
        Task<GetAllVideosResponseModel> GetAllVideos(GetAllVideosRequestModel videoRequestModel);

        Task<GetVideoDetailsResponseModel> GetVideoDetails(GetVideoDetailsRequestModel videoRequestModel);
        Task SaveVideo(UploadVideoRequestModel videoRequestModel);
        public Task<UploadVideoResponseModel> UploadVideoToS3BucketAsync(UploadVideoRequestModel requestDto);
    }
}