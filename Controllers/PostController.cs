using Click_Go.DTOs;
using Click_Go.Models; 
using Click_Go.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; 
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic; 

namespace Click_Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostController> _logger;

        public PostController(IPostService postService, ILogger<PostController> logger)
        {
            _postService = postService;
            _logger = logger; 
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(PostReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] 
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

            var createdPost = await _postService.CreatePostAsync(postDto, userId);
            var postReadDto = MapPostToReadDto(createdPost);

            _logger.LogInformation("Post created successfully with ID: {PostId}", postReadDto.Id);
            return CreatedAtAction(nameof(GetPostById), new { id = postReadDto.Id }, postReadDto);
        }

        [HttpGet("{id}")]//GetPostByPostID
        [ProducesResponseType(typeof(PostReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public async Task<IActionResult> GetPostById(long id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            return Ok(post);
        }

        [HttpGet("MyPosts")] // Get current user's posts
        [ProducesResponseType(typeof(IEnumerable<GetPostDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMyPosts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("GetMyPosts attempt failed: User ID not found in token.");
                return Unauthorized(new ProblemDetails { Title = "Unauthorized", Detail = "User ID not found in token." });
            }

            var postsWithDetails = await _postService.GetPostsByUserIdAsync(userId);

            _logger.LogInformation("Retrieved {Count} posts for user {UserId}", postsWithDetails.Count(), userId);
            return Ok(postsWithDetails);
        }

        [HttpGet("search/address")]
        [AllowAnonymous] // Hoặc [Authorize] tùy theo yêu cầu của bạn
        [ProducesResponseType(typeof(IEnumerable<PostReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Nếu query không hợp lệ
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchPostsByAddress([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new ProblemDetails { Title = "Bad Request", Detail = "Search query cannot be empty." });
            }

            _logger.LogInformation("Attempting to search posts with address query: {Query}", query);
            
            var result = await _postService.SearchByAddressAsync(query);
            return Ok(result);
        }

        [HttpGet("GetAllPosts")]
        [AllowAnonymous] // Allow public access to view all posts
        [ProducesResponseType(typeof(IEnumerable<PostReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllPosts()
        {
            _logger.LogInformation("Retrieving all posts");
            var posts = await _postService.GetAllPostsAsync();
            return Ok(posts);
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
