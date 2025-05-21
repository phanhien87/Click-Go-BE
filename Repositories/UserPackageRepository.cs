using Click_Go.Data;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Click_Go.Repositories
{
    public class UserPackageRepository : IUserPackageRepository
    {
        private readonly ApplicationDbContext _context;

        public UserPackageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserPackage userPackage)
        {
            _context.UserPackages.Add(userPackage);
            await _context.SaveChangesAsync();

            
        }

        public async Task<List<UserPackage>> GetAllAsync()
        {
            return await _context.UserPackages.ToListAsync();
        }
    }
}
