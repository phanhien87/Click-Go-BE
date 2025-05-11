using Microsoft.AspNetCore.Hosting;

namespace Click_Go.Helper
{
    public class SaveImage
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SaveImage(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subfolder)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            // Use ContentRootPath instead of WebRootPath
            var baseFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "UploadedFiles");
            var specificUploadsFolder = Path.Combine(baseFolder, subfolder);

            if (!Directory.Exists(specificUploadsFolder))
            {
                Directory.CreateDirectory(specificUploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(specificUploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the relative path starting from "UploadedFiles"
            return Path.Combine("UploadedFiles", subfolder, uniqueFileName).Replace("\\", "/");
        }
    }
}
