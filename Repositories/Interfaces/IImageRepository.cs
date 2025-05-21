using Click_Go.Models;

namespace Click_Go.Repositories.Interfaces
{
    public interface IImageRepository
    {
        Task AddImagesAsync(IEnumerable<Image> images);
    }
}
