using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Click_Go.Services.Interfaces;

namespace Click_Go.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task AddAsync(Notification notification)
        {
           await _notificationRepository.AddAsync(notification);
        }

        public async Task<List<Notification>> GetAllByUserIdAsync(string userId)
        {
            return await _notificationRepository.GetAllNotificationByUserId(userId);
        }

        public async Task<Notification> GetByIdAsync(long id)
        {
          return await _notificationRepository.GetByIdAsync(id);    
        }

        public async Task UpdateAsync(Notification notification)
        {
           await _notificationRepository.UpdateAsync(notification); 
        }

    }
}
