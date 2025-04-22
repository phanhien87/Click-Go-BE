using Click_Go.DTOs;
using Click_Go.Models;
using Click_Go.Services;
using Click_Go.Services.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Click_Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IReviewService _reviewService;
        private readonly IReactService _reactService;
        public CommentController(ICommentService commentService, IReviewService reviewService, IReactService reactService)
        {
            _commentService = commentService;
            _reviewService = reviewService;
            _reactService = reactService;
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
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid user" });

            await _commentService.AddCommentAsync(commentDto, userId);
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
    }
}