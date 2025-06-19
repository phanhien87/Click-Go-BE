using System.ComponentModel.Design;
using System.Net.WebSockets;
using System.Security.Claims;
using Click_Go.DTOs;
using Click_Go.Hubs;
using Click_Go.Models;
using Click_Go.Services;
using Click_Go.Services.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Click_Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IReviewService _reviewService;
        private readonly IReactService _reactService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;
        public CommentController(ICommentService commentService, IReviewService reviewService, IReactService reactService, INotificationService notificationService, IHubContext<NotificationHub> hubContext)
        {
            _commentService = commentService;
            _reviewService = reviewService;
            _reactService = reactService;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        [HttpPost("review")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> Review([FromForm] ReviewRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid user" });

            await _reviewService.AddReviewAsync(dto, userId);
            return Ok(new { message = "Review submitted successfully!" });
        }

        [HttpPost("reply")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> Reply([FromForm] CommentDto commentDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserName = User.Identity?.Name;
            var noti = new Notification();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid user" });

            var commentId =  await _commentService.AddCommentAsync(commentDto, userId);

            var parentComment = await _commentService.GetParentCmtById(commentDto.ParentId);
            if (parentComment != null && parentComment.UserId != userId)
            {
           
                noti = new Notification
                {
                    UserId = parentComment.UserId,
                    SenderId = userId,
                    Message = $"{currentUserName} đã phản hồi bình luận của bạn",
                    Url = $"/Post/{commentDto.PostId}#comment-{commentId}",
                    Status = 1,
                    IsRead = false,
                    CreatedDate = DateTime.Now,
                };
                await _notificationService.AddAsync(noti);
            }

            await _hubContext.Clients.Group(parentComment.UserId)
              .SendAsync("ReceiveNotification", new
              {
                  message = noti.Message,
                  url = noti.Url
              });

            return Ok(new { message = "Reply submitted successfully!" });
        }

        [HttpPost("react")]
        [Authorize(Roles ="CUSTOMER")]
        public async Task<IActionResult> React([FromBody] ReactDto reactDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid user" });

            await _reactService.ReactComment(reactDto, userId);
            return Ok(new { message = "react submitted successfully!" });
        }

        [HttpGet("allcomments")]
        public async Task<IActionResult> GetAllCommnets([FromQuery] long postId, [FromQuery] long? parentCommentId = null, [FromQuery] int level = int.MaxValue)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allComments = await _commentService.GetCommentsByPostAsync(postId, userId);
            return Ok(allComments);
        }
    }
}