using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
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
        public UploadsController(ILogger<UploadsController> logger, IStorageService storageService, DaprClient daprClient)
        {
            _logger = logger;
            _storageService = storageService;
            _daprClient = daprClient;
        }

        [HttpGet(Name = "GetAllUploadedVideos")]
        public  async Task<IActionResult> GetAll()
        {
           var response =  await _storageService.GetAllVideos(new Services.Models.GetAllVideosRequestModel());
            return Ok(response);
        }

        [HttpPost(Name = "SaveVideos")]
        public async Task<IActionResult> Post([FromForm] UploadVideoRequestModel uploadVideoRequest)
        {
            if (!Validations.IsValidVideoFile(uploadVideoRequest.File))
            {
                return BadRequest("Invalid file, .mp4 is allowed with max size for 102400");
            }

           var key =  await _storageService.UploadVideoToS3BucketAsync(uploadVideoRequest);

            var reponse = new UploadVideoResponseModel()
            {
                Bucketkey = key
            };

            await _daprClient.PublishEventAsync<UploadVideoResponseModel>("pubsub", "upload", reponse);

            return Ok(reponse);
        }
    }


}