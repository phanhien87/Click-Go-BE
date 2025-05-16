using Click_Go.Data;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Click_Go.Repositories
{
    public class PackageRepository : IPackageRepository
    {
        private readonly ApplicationDbContext _context;

        public PackageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Package>> GetAllPackagesAsync()
        {
            return await _context.Packages.ToListAsync();
        }

        public async Task<Package> GetPackageByIdAsync(long id)
        {
            return await _context.Packages.FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
