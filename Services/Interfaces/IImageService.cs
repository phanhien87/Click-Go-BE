using Click_Go.Models;

namespace Click_Go.Services.Interfaces
{
    public interface IImageService
    {
        Task AddImageAsync(IEnumerable<Image> images);
    }
}
