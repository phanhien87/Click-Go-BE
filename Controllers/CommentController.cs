using Click_Go.DTOs;
using Click_Go.Models;
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
        
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
           
        }
        [HttpPost]
        [Authorize("CUSTOMER")]
        public async Task<IActionResult> AddComment([FromForm] CommentDto commentDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _commentService.AddCommentAsync(commentDto, userId);

            return Ok(new { message = "Comment successfully!" });
        }
    }
}