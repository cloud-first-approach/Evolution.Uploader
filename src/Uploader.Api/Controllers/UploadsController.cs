using Microsoft.AspNetCore.Mvc;
using Uploader.Api.Models;

namespace Uploader.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadsController : ControllerBase
    {

        private readonly ILogger<UploadsController> _logger;

        public UploadsController(ILogger<UploadsController> logger)
        {
            _logger = logger;
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
        public async Task<IActionResult> Post(UploadVideoRequestModel model)
        {
            return Ok();
        }
    }
}