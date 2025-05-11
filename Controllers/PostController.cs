using Click_Go.DTOs;
using Click_Go.Models; // Needed for mapping
using Click_Go.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Add logger namespace
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic; // For List

namespace Click_Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for all actions in this controller
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostController> _logger; // Inject logger

        public PostController(IPostService postService, ILogger<PostController> logger)
        {
            _postService = postService;
            _logger = logger; // Assign logger
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(PostReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePost([FromForm] PostCreateDto postDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("CreatePost attempt failed: User ID not found in token.");
                return Unauthorized(new ProblemDetails { Title = "Unauthorized", Detail = "User ID not found in token." });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CreatePost validation failed: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                var createdPost = await _postService.CreatePostAsync(postDto, userId);

                // **Map Entity to DTO**
                var postReadDto = MapPostToReadDto(createdPost);

                _logger.LogInformation("Post created successfully with ID: {PostId}", postReadDto.Id);
                return CreatedAtAction(nameof(GetPostById), new { id = postReadDto.Id }, postReadDto);
            }
            catch (ArgumentException ex) // Specific exception from service
            {
                _logger.LogError(ex, "Argument error during post creation for user {UserId}. DTO: {@PostDto}", userId, postDto);
                return BadRequest(new ProblemDetails { Title = "Invalid Input", Detail = ex.Message });
            }
            catch (Exception ex) // Catch broader exceptions
            {
                _logger.LogError(ex, "An unexpected error occurred while creating post for user {UserId}. DTO: {@PostDto}", userId, postDto);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails { Title = "Internal Server Error", Detail = "An unexpected error occurred. Please try again later." });
            }
        }

        
        [HttpGet("{id}")]//GetPostByPostID
        [ProducesResponseType(typeof(PostReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPostById(long id)
        {
            try
            {
                var post = await _postService.GetPostByIdAsync(id);

                if (post == null)
                {
                    _logger.LogInformation("GetPostById request for ID {PostId} returned null.", id);
                    return NotFound(new ProblemDetails { Title = "Not Found", Detail = $"Post with ID {id} not found." });
                }

                // Map Entity to DTO
                var postReadDto = MapPostToReadDto(post);

                return Ok(postReadDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving post with ID {PostId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails { Title = "Internal Server Error", Detail = "An unexpected error occurred. Please try again later." });
            }
        }

        [HttpGet("MyPosts")] // Get current user's posts
        [ProducesResponseType(typeof(IEnumerable<PostReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMyPosts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("GetMyPosts attempt failed: User ID not found in token.");
                return Unauthorized(new ProblemDetails { Title = "Unauthorized", Detail = "User ID not found in token." });
            }

            try
            {
                var posts = await _postService.GetPostsByUserIdAsync(userId);

                // Map the list of entities to a list of DTOs
                var postReadDtos = posts.Select(post => MapPostToReadDto(post)).ToList();

                _logger.LogInformation("Retrieved {Count} posts for user {UserId}", postReadDtos.Count, userId);
                return Ok(postReadDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving posts for user {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails { Title = "Internal Server Error", Detail = "An unexpected error occurred. Please try again later." });
            }
        }

        // --- Helper Method for Mapping --- 
        private PostReadDto MapPostToReadDto(Post post)
        {
            // Handle potential null navigation properties
            return new PostReadDto
            {
                Id = post.Id,
                Name = post.Name,
                Title = post.Title,
                Logo_Image = post.Logo_Image,
                Background = post.Background,
                SDT = post.SDT,
                Address = post.Address,
                Description = post.Description,
                CreatedDate = post.CreatedDate,
                UpdatedDate = post.UpdatedDate,
                Category = post.Category != null ? new CategoryDto
                {
                    Id = post.Category.Id,
                    Name = post.Category.Name
                } : null,
                User = post.User != null ? new UserDto
                {
                    Id = post.User.Id,
                    UserName = post.User.UserName,
                    FullName = post.User.FullName
                } : null,
                OpeningHours = post.Opening_Hours?.Select(oh => new OpeningHourDto
                {
                    DayOfWeek = oh.DayOfWeek,
                    OpenHour = oh.OpenHour ?? 0, // Provide default if nullable
                    OpenMinute = oh.OpenMinute ?? 0,
                    CloseHour = oh.CloseHour ?? 0,
                    CloseMinute = oh.CloseMinute ?? 0
                }).ToList() ?? new List<OpeningHourDto>(),
                Images = post.Images?.Select(img => new ImageDto
                {
                    Id = img.Id,
                    ImagePath = img.ImagePath
                }).ToList() ?? new List<ImageDto>()
            };
        }
    }
}
