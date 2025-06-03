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

        public async Task<UserPackage> CheckPackageByUserId(string userId)
        {
               return await _context.UserPackages.Where(up => up.Status == 1).FirstOrDefaultAsync(up => up.UserId == userId);    
        }

        public async Task<List<UserPackage>> GetAllAsync()
        {
            return await _context.UserPackages.ToListAsync();
        }

        public async Task UpdateAsync(UserPackage userPackage)
        {
            if (userPackage == null) return;
            if (_context.Entry(userPackage).State == EntityState.Detached)
            {
                _context.UserPackages.Attach(userPackage);
                _context.Entry(userPackage).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
        }
    }
}
