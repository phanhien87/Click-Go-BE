using Click_Go.Models;

namespace Click_Go.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);

        Task<List<Notification>> GetAllNotificationByUserId(string userId);
        Task <Notification> GetByIdAsync(long id);
    }
}
