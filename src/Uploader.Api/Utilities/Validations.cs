namespace Uploader.Api.Utilities
{
    public static class Validations
    {
        public static bool IsValidVideoFile(IFormFile file)
        {

            // Check file length
            if (file.Length < 0)
            {
                return false;
            }

            // Check file extension to prevent security threats associated with unknown file types
            string[] permittedExtensions = new string[] { ".mp4" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains<string>(ext))
            {
                return false;
            }

            // Check if file size is greater than permitted limit
            if (file.Length > 1024000) // 6MB
            {
                return false;
            }

            return true;
        }
    }
}
