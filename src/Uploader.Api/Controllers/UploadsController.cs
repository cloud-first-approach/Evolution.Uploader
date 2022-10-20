using Dapr.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Uploader.Api.AppSettings;
using Uploader.Api.Models;
using Uploader.Api.Services;
using Uploader.Api.Utilities;

namespace Uploader.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadsController : ControllerBase
    {

        private readonly ILogger<UploadsController> _logger;
        private readonly IStorageService _storageService;
        private readonly DaprClient _daprClient;
        private readonly StorageSettings _storageSettings;
        public UploadsController(ILogger<UploadsController> logger, IStorageService storageService, DaprClient daprClient, IOptions<StorageSettings> storageSettings)
        {
            _logger = logger;
            _storageService = storageService;
            _daprClient = daprClient;
            _storageSettings = storageSettings.Value;
        }
        
        [Route("all")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _storageService.GetAllVideos(new Services.Models.GetAllVideosRequestModel());
            return Ok(response);
        }

        [Route("video")]
        [HttpGet(Name = "GetVideoDetail")]
        public async Task<IActionResult> Get([FromQuery] string key, [FromQuery] string bucketName)
        {
            var response = await _storageService.GetVideoDetails(new Services.Models.GetVideoDetailsRequestModel() { Key = key,BucketName = bucketName});
            return Ok(response);
        }

        [HttpPost(Name = "SaveVideo")]
        public async Task<IActionResult> Post([FromForm] UploadVideoRequestModel uploadVideoRequest)
        {
            if (!Validations.IsValidVideoFile(uploadVideoRequest.File))
            {
                return BadRequest("Invalid file, .mp4 is allowed with max size for 102400");
            }

            uploadVideoRequest.BucketFolder = _storageSettings.BucketFolder;
            uploadVideoRequest.BucketName = _storageSettings.BucketName;

            var reponse = await _storageService.UploadVideoToS3BucketAsync(uploadVideoRequest);

            await _daprClient.PublishEventAsync<UploadVideoResponseModel>("pubsub", "upload", reponse);

            return Ok(reponse);
        }
    }


}