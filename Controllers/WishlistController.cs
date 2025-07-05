using Click_Go.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Click_Go.DTOs;

namespace Click_Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for all actions
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(IWishlistService wishlistService, ILogger<WishlistController> logger)
        {   
            _wishlistService = wishlistService;
            _logger = logger;
        }

        // POST api/wishlist/{postId}
        [HttpPost("{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]                 
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]           
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public async Task<IActionResult> ToggleWishlist(long postId)
        {   
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            _logger.LogInformation("User {UserId} attempting to toggle wishlist status for post {PostId}", userId, postId);
            
            var added = await _wishlistService.ToggleWishlistAsync(userId, postId);

            if (added)
            {   
                _logger.LogInformation("Post {PostId} added to wishlist for user {UserId}", postId, userId);
                return Ok(new { message = "Post added to wishlist." });
            }
            else
            {   
                _logger.LogInformation("Post {PostId} removed from wishlist for user {UserId}", postId, userId);
                return Ok(new { message = "Post removed from wishlist." });
            }
        }

        // GET api/wishlist
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PostReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public async Task<IActionResult> GetUserWishlist()
        {   
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation("User {UserId} fetching their wishlist", userId);
            var wishlistPosts = await _wishlistService.GetUserWishlistAsync(userId);
            return Ok(wishlistPosts);
        }
    }
} 