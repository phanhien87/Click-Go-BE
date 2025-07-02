using Click_Go.Data;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Click_Go.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);    
            await _context.SaveChangesAsync();  
        }

        public async Task<List<Notification>> GetAllNotificationByUserId(string userId)
        {
           return await _context.Notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedDate).ToListAsync();
        }

        public async Task<Notification> GetByIdAsync(long id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public async Task UpdateAsync(Notification notification)
        {
             _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }
    }
}
