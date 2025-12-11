using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Click_Go.DTOs;
using Click_Go.Helper;
using Click_Go.Models; 
using Click_Go.Services.Interfaces;
using Confluent.Kafka;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; 


namespace Click_Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(Enum.Application_Role.CUSTOMER))] 
    public class PostController : ControllerBase
    {
        private readonly IProducer<Null, string> _kafkaProducer;
        private readonly IPostService _postService;
        private readonly ILogger<PostController> _logger;

        public PostController(IPostService postService, ILogger<PostController> logger, IProducer<Null, string> kafkaProducer)
        {
            _postService = postService;
            _logger = logger;
            _kafkaProducer = kafkaProducer;
        }
        [HttpGet]
        public async Task<IActionResult> GetPostIdByUser([FromQuery] string userId)
        {
            var idPost = await _postService.GetPostIdByUserAsync(userId);
            if(idPost == null) return NotFound();
            return Ok(idPost);
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
            _logger.LogInformation("Attempting to retrieve post with ID: {PostId}", id);
            try
            {
                var getPostDto = await _postService.GetPostByIdAsync(id);
                _logger.LogInformation("Successfully retrieved post with ID: {PostId}", id);
                return Ok(getPostDto);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Post not found: {Message}", ex.Message);
                return NotFound(new ProblemDetails { Title = "Not Found", Detail = ex.Message });
            }
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

        [HttpGet("search")] 
        [AllowAnonymous]
        [ProducesResponseType(typeof(PaginationDto<PostReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchPosts([FromQuery] PostSearchDto searchDto)
        {
            try
            {
                // Validate that at least one search parameter is provided
                if (searchDto == null)
                {
                    return BadRequest(new ProblemDetails 
                    { 
                        Title = "Bad Request", 
                        Detail = "Search criteria cannot be null."
                    });
                }

                bool hasSearchCriteria = 
                    !string.IsNullOrWhiteSpace(searchDto.PostName) ||
                    !string.IsNullOrWhiteSpace(searchDto.District) || 
                    !string.IsNullOrWhiteSpace(searchDto.Ward) || 
                    !string.IsNullOrWhiteSpace(searchDto.City) ||
                    (searchDto.TagNames != null && searchDto.TagNames.Any(t => !string.IsNullOrWhiteSpace(t))) ||
                    searchDto.MinPrice.HasValue ||
                    searchDto.MaxPrice.HasValue;

                if (!hasSearchCriteria)
                {
                    return BadRequest(new ProblemDetails 
                    { 
                        Title = "Bad Request", 
                        Detail = "At least one search parameter must be provided."
                    });
                }

                // Validate price range if both min and max are provided
                if (searchDto.MinPrice.HasValue && searchDto.MaxPrice.HasValue && 
                    searchDto.MinPrice.Value > searchDto.MaxPrice.Value)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Bad Request",
                        Detail = "Minimum price cannot be greater than maximum price."
                    });
                }

                // Validate pagination parameters
                if (searchDto.PageNumber < 1)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Bad Request",
                        Detail = "Page number must be greater than or equal to 1."
                    });
                }

                if (searchDto.PageSize < 1)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Bad Request",
                        Detail = "Page size must be greater than or equal to 1."
                    });
                }

                _logger.LogInformation("Attempting to search posts with criteria: {@SearchCriteria}", searchDto);
                
                var result = await _postService.SearchPostsAsync(searchDto);
                
                if (result.TotalItems == 0)
                {
                    return NotFound(new ProblemDetails 
                    { 
                        Title = "Not Found", 
                        Detail = "No posts matched your search criteria or no active posts were found." 
                    });
                }
                //await _kafkaProducer.ProduceAsync("search-queries", new Message<Null, string> { Value = searchDto.PostName });
                return Ok(result);
            }
            catch (AppException ex) when (ex.Message.Contains("Page number") && ex.Message.Contains("exceeds total pages"))
            {
                _logger.LogWarning("Invalid pagination request: {Message}", ex.Message);
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Page Number",
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching posts");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ProblemDetails
                    {
                        Title = "Internal Server Error",
                        Detail = "An error occurred while processing your request."
                    });
            }
        }

        [HttpGet("GetAllPosts")]
        [AllowAnonymous] // Allow public access to view all posts
        [ProducesResponseType(typeof(IEnumerable<PostReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllPosts()
        {
            _logger.LogInformation("Retrieving all active posts");
            var posts = await _postService.GetAllPostsAsync();
            
            if (posts == null || !posts.Any())
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Not Found",
                    Detail = "No active posts were found."
                });
            }
            
            return Ok(posts);
        }

        [HttpPut]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(PostReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePost([FromForm] PostUpdateDto postDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("UpdatePost attempt failed: User ID not found in token.");
                return Unauthorized(new ProblemDetails { Title = "Unauthorized", Detail = "User ID not found in token." });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("UpdatePost validation failed: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                var updatedPost = await _postService.UpdatePostAsync(postDto, userId);
                var postReadDto = MapPostToReadDto(updatedPost);

                _logger.LogInformation("Post updated successfully with ID: {PostId}", postReadDto.Id);
                return Ok(postReadDto);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("UpdatePost not found: {Message}", ex.Message);
                return NotFound(new ProblemDetails { Title = "Not Found", Detail = ex.Message });
            }
            catch (AppException ex)
            {
                _logger.LogWarning("UpdatePost forbidden: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status403Forbidden, 
                    new ProblemDetails { Title = "Forbidden", Detail = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating post");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ProblemDetails { Title = "Internal Server Error", Detail = "An error occurred while updating the post." });
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
                Price = post.Price,
                LinkFacebook = post.LinkFacebook,
                LinkGoogleMap = post.LinkGoogleMap,
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
                }).ToList() ?? new List<ImageDto>(),
                Tags = post.Tags?.Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList() ?? new List<TagDto>()
            };
        }
    }
}
