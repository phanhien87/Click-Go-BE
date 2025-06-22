using System.Security.Claims;
using Click_Go.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Click_Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var notifs = await _notificationService.GetAllByUserIdAsync(userId);
            return Ok(notifs);
        }

        [HttpPost("{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(long id)
        {
            var notif = await _notificationService.GetByIdAsync(id);
            if (notif == null) return NotFound();
            notif.IsRead = true;
            await _notificationService.UpdateAsync(notif);
            
            return Ok();
        }
    }
}
