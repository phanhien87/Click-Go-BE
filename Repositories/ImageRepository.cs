using Click_Go.Data;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;

namespace Click_Go.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddImagesAsync(IEnumerable<Image> images)
        {
            await _context.Images.AddRangeAsync(images);
            await _context.SaveChangesAsync();
        }
    }
}
