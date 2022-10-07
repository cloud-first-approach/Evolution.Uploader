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

        public UploadsController(ILogger<UploadsController> logger, IStorageService storageService)
        {
            _logger = logger;
            _storageService = storageService;
        }

        [HttpGet(Name = "GetVideos")]
        public IEnumerable<UploadVideoRequestModel> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new UploadVideoRequestModel
            {
                ActualCreatedDate = DateTime.Now.AddDays(index),
            })
            .ToArray();
        }

        [HttpPost(Name = "SaveVideos")]
        public async Task<IActionResult> Post([FromForm] UploadVideoRequestModel uploadVideoRequest)
        {
            if (!Validations.IsValidVideoFile(uploadVideoRequest.File))
            {
                return BadRequest("Invalid file, .mp4 is allowed with max size for 102400");
            }

            await _storageService.SaveVideo(uploadVideoRequest);

            return Ok(new { uploadVideoRequest.File.Length });
        }
    }


}