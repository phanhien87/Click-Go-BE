using Click_Go.Models;

namespace Click_Go.Services.Interfaces
{
    public interface INotificationService
    {
        Task AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task<List<Notification>> GetAllByUserIdAsync(string userId);
        Task<Notification> GetByIdAsync(long id);
    }
}
