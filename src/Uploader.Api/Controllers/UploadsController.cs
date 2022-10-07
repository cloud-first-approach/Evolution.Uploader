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
        public async Task<IActionResult> Post([FromForm] UploadVideoRequestModel uploadVideoRequest)
        {
            var files = Request.Form.Files;
            if (!files.Any())
            {
                return  BadRequest("No File found to Upload");
            }
            
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.GetTempFileName();

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size });
        }
    }
}