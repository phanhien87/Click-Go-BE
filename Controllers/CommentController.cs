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

            var reviewResponse = await _reviewService.AddReviewAsync(dto, userId);
            return Ok(reviewResponse);
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

            var comment = await _commentService.AddCommentAsync(commentDto, userId);

            var parentComment = await _commentService.GetParentCmtById(commentDto.ParentId);
            if (parentComment != null && parentComment.UserId != userId)
            {

                noti = new Notification
                {
                    UserId = parentComment.UserId,
                    SenderId = userId,
                    Message = $"{currentUserName} đã phản hồi bình luận của bạn",
                    Url = $"/Post/{commentDto.PostId}#comment-{comment?.CommentId}",
                    Status = 1,
                    CommentId = comment?.CommentId,
                    IsRead = false,
                    CreatedDate = DateTime.Now,
                };
                await _notificationService.AddAsync(noti);
            }

            await _hubContext.Clients.User(parentComment.UserId)
              .SendAsync("ReceiveNotification", new
              {
                  message = noti.Message,
                  url = noti.Url
              });

            return Ok(comment);
        }

        [HttpPost("react")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> React([FromBody] ReactDto reactDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid user" });

            await _reactService.ReactComment(reactDto, userId);
            return Ok(new { message = "react submitted successfully!" });
        }

        [HttpGet]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetComments(
                                                    [FromQuery] long postId,
                                                    [FromQuery] long? parentId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var comments = await _commentService.GetCommentsByPostAndParentAsync(postId, parentId, userId);
            return Ok(comments);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> DeleteComment(long id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var (success, isRootComment, newParentId) = await _commentService.DeleteCommentAsync(id, userId);
            if (!success)
            {
                return NotFound("Comment not found or user not authorized to delete.");
            }

            var message = isRootComment
                ? "Root comment and all its replies deleted successfully."
                : "Reply deleted successfully.";
            return Ok(new { Message = message, NewParentID = newParentId});
        }

        [HttpGet("{commentId}/ancestor-path")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetCommentAncestorPath(long commentId)
        {

            var result = await _commentService.GetCommentAncestorPathAsync(commentId);

            if (!result.Exists)
            {
                return NotFound(new
                {
                    exists = false,
                    message = "Comment not found"
                });
            }

            return Ok(new
            {
                exists = true,
                ancestorPath = result.AncestorPath,
                rootCommentId = result.RootCommentId,
                depth = result.Depth
            });
        }

    }
}

