using Click_Go.Models;
using Click_Go.Repositories;
using Click_Go.Repositories.Interfaces;
using Click_Go.Services.Interfaces;

namespace Click_Go.Services
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;

        public ImageService(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public async Task AddImageAsync(IEnumerable<Image> images)
        {
            await _imageRepository.AddImagesAsync(images);
        }
    }
}
