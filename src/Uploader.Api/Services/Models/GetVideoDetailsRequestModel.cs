namespace Uploader.Api.Services.Models
{
    public class GetVideoDetailsRequestModel
    {
        public string Key { get; set; }
        public string BucketName { get; internal set; }
    }
}
