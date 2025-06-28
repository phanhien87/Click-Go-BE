using Click_Go.Data;
using Click_Go.DTOs;
using Click_Go.Models;
using Click_Go.Helper;
using Click_Go.Services.Interfaces;
using Click_Go.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Click_Go.Services
{
    public class PostService : IPostService
    {
        private readonly SaveImage _saveImageHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPostRepository _postRepository;
        private readonly ICommentService _commentService;
        private readonly IRatingRepository _ratingRepository;
        private readonly ICommentRepository _commentRepository;
        public PostService(
            IWebHostEnvironment webHostEnvironment, 
            SaveImage saveImageHelper, 
            IPostRepository postRepository,
            IRatingRepository ratingRepository,
            ICommentService commentService,
            ICommentRepository commentRepository)
        {
            _webHostEnvironment = webHostEnvironment;
            _saveImageHelper = saveImageHelper;
            _postRepository = postRepository;
            _commentService = commentService;
            _ratingRepository = ratingRepository;
            _commentRepository = commentRepository;
        }

        public async Task<Post> CreatePostAsync(PostCreateDto postDto, string userId)
        {
            var userPackage = await _postRepository.GetUserPackageByUserIdAsync(userId);
            if (userPackage == null)
            {
                throw new AppException("User does not have an active package. Please subscribe to a package to post.");
            }

            if (userPackage.ExpireDate < DateTime.UtcNow)
            {
                throw new AppException("Your package has expired. Please renew your subscription to post.");
            }

            var existingPosts = await _postRepository.GetByUserIdAsync(userId);
            if (existingPosts.Any())
            {
                throw new AppException("User already has a post.");
            }

            if (string.IsNullOrWhiteSpace(postDto.Name) || postDto.CategoryId <= 0)
            {
                throw new AppException("Post name and category ID are required.");
            }

            var category = await _postRepository.GetCategoryByIdAsync(postDto.CategoryId);
            if (category == null)
            {
                throw new NotFoundException("Invalid Category ID.");
            }

            // Combine address components
            var addressParts = new List<string?>();
            if (!string.IsNullOrWhiteSpace(postDto.Street))
            {
                addressParts.Add(postDto.Street.Trim());
            }
            if (!string.IsNullOrWhiteSpace(postDto.Ward))
            {
                addressParts.Add(postDto.Ward.Trim());
            }
            if (!string.IsNullOrWhiteSpace(postDto.District))
            {
                addressParts.Add(postDto.District.Trim());
            }
            if (!string.IsNullOrWhiteSpace(postDto.City))
            {
                addressParts.Add(postDto.City.Trim());
            }
            var combinedAddress = string.Join(", ", addressParts.Where(s => !string.IsNullOrEmpty(s)));

            var post = new Post
            {
                Name = postDto.Name,
                Title = postDto.Title,
                SDT = postDto.SDT,
                Address = combinedAddress,
                Description = postDto.Description,
                Price = postDto.Price,
                LinkFacebook = postDto.LinkFacebook,
                LinkGoogleMap = postDto.LinkGoogleMap,
                CategoryId = postDto.CategoryId,
                UserId = userId,
                Images = new List<Image>(),
                Opening_Hours = new List<OpeningHour>(),
                CreatedDate = DateTime.UtcNow,
                Status = 1,
                CreatedUser = Guid.Parse(userId)
            };

            if (postDto.TagIds != null && postDto.TagIds.Any())
            {
                var tags = await _postRepository.GetTagsByIdsAsync(postDto.TagIds);
                post.Tags = tags;
            }

            if (postDto.LogoImage != null)
            {
                post.Logo_Image = await _saveImageHelper.SaveFileAsync(postDto.LogoImage, "logos");
            }

            if (postDto.BackgroundImage != null)
            {
                post.Background = await _saveImageHelper.SaveFileAsync(postDto.BackgroundImage, "backgrounds");
            }

            if (postDto.OtherImages != null && postDto.OtherImages.Any())
            {
                foreach (var imageFile in postDto.OtherImages)
                {
                    var imagePath = await _saveImageHelper.SaveFileAsync(imageFile, "posts");
                    post.Images.Add(new Image
                    {
                        ImagePath = imagePath,
                        CommentId = null,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow
                    });
                }
            }

            if (postDto.OpeningHours != null && postDto.OpeningHours.Any())
            {
                foreach (var ohDto in postDto.OpeningHours)
                {
                    if (string.IsNullOrWhiteSpace(ohDto.DayOfWeek) ||
                        ohDto.OpenHour < 0 || ohDto.OpenHour > 23 || ohDto.OpenMinute < 0 || ohDto.OpenMinute > 59 ||
                        ohDto.CloseHour < 0 || ohDto.CloseHour > 23 || ohDto.CloseMinute < 0 || ohDto.CloseMinute > 59)
                    {
                        continue;
                    }

                    post.Opening_Hours.Add(new OpeningHour
                    {
                        DayOfWeek = ohDto.DayOfWeek,
                        OpenHour = ohDto.OpenHour,
                        OpenMinute = ohDto.OpenMinute,
                        CloseHour = ohDto.CloseHour,
                        CloseMinute = ohDto.CloseMinute,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow
                    });
                }
            }

            return await _postRepository.CreateAsync(post);
        }

        private double CalculateAverageStars(OverallCriteriaDto overallCriteria)
        {
            if (overallCriteria == null) return 0;
            var scores = new List<double> { overallCriteria.Quality, overallCriteria.Location, overallCriteria.Space, overallCriteria.Service, overallCriteria.Price };
           // var validScores = scores.Where(s => s > 0).ToList(); // Consider only scores > 0 if that's the logic
            return scores.Any() ? scores.Average() : 0;
        }

        public async Task<GetPostDto> GetPostByIdAsync(long id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                throw new NotFoundException($"Post with ID {id} not found.");
            }

            //var comments = await _commentService.GetCommentsByPostAsync(id);
            var overallRating = await _ratingRepository.GetOverallCriteriaByPostId(id);

            int totalComments = await _commentRepository.GetTotalCommentByPost(id);
            double averageStars = CalculateAverageStars(overallRating);

            var postReadDto = MapPostToReadDto(post, totalComments, averageStars);
            
            return new GetPostDto 
            { 
                Post = postReadDto, 
                //Comment = comments?.ToList() ?? new List<GetCommentByPostDto>(), 
                Rating = overallRating 
            };
        }


        public async Task<IEnumerable<GetPostDto>> GetPostsByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("User ID cannot be null or empty when fetching user posts.");
            }

            var posts = await _postRepository.GetByUserIdAsync(userId);
            if (posts == null || !posts.Any())
            {
                return Enumerable.Empty<GetPostDto>();
            }

            var resultList = new List<GetPostDto>();
            foreach (var post in posts)
            {
               // var comments = await _commentService.GetCommentsByPostAsync(post.Id);
                var overallRating = await _ratingRepository.GetOverallCriteriaByPostId(post.Id);

                int totalComments = await _commentRepository.GetTotalCommentByPost(post.Id);
                double averageStars = CalculateAverageStars(overallRating);

                var postReadDto = MapPostToReadDto(post, totalComments, averageStars);

                resultList.Add(new GetPostDto
                {
                    Post = postReadDto,
                    //Comment = comments?.ToList() ?? new List<GetCommentByPostDto>(),
                    Rating = overallRating
                });
            }
            return resultList;
        }

        private PostReadDto MapPostToReadDto(Post post, int totalComments, double averageStars)
        {
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
                }).ToList() ?? new List<TagDto>(),
                TotalComments = totalComments,
                AverageStars = averageStars
            };
        }
        public async Task<PaginationDto<PostReadDto>> SearchPostsAsync(PostSearchDto searchDto)
        {
            // Validate search criteria
            if (searchDto == null)
            {
                return new PaginationDto<PostReadDto>
                {
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 0,
                    TotalItems = 0,
                    Items = Enumerable.Empty<PostReadDto>()
                };
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
                return new PaginationDto<PostReadDto>
                {
                    PageNumber = searchDto.PageNumber,
                    PageSize = searchDto.PageSize,
                    TotalPages = 0,
                    TotalItems = 0,
                    Items = Enumerable.Empty<PostReadDto>()
                };
            }

            // Clean up tag names if provided
            if (searchDto.TagNames != null)
            {
                // Filter out empty tags
                searchDto.TagNames = searchDto.TagNames
                    .Where(tn => !string.IsNullOrWhiteSpace(tn))
                    .Select(tn => tn.Trim())
                    .Distinct()
                    .ToList();
                
                // If all tags were empty, set to null
                if (!searchDto.TagNames.Any())
                {
                    searchDto.TagNames = null;
                }
            }

            // Ensure valid pagination parameters
            if (searchDto.PageNumber < 1)
            {
                searchDto.PageNumber = 1;
            }

            if (searchDto.PageSize < 1)
            {
                searchDto.PageSize = 10;
            }
            else if (searchDto.PageSize > 50) // Limit max page size
            {
                searchDto.PageSize = 50;
            }

            // Get total count of matching posts
            int totalCount = await _postRepository.CountSearchResultsAsync(searchDto);

            // Calculate total pages
            int totalPages = (int)Math.Ceiling(totalCount / (double)searchDto.PageSize);

            // If no posts match, return empty result
            if (totalCount == 0)
            {
                return new PaginationDto<PostReadDto>
                {
                    PageNumber = searchDto.PageNumber,
                    PageSize = searchDto.PageSize,
                    TotalPages = 0,
                    TotalItems = 0,
                    Items = Enumerable.Empty<PostReadDto>()
                };
            }

            // Check if requested page exceeds total pages
            if (searchDto.PageNumber > totalPages)
            {
                throw new AppException($"Page number {searchDto.PageNumber} exceeds total pages {totalPages}. Valid page range is 1 to {totalPages}.");
            }

            // Get paginated posts
            var posts = await _postRepository.SearchPostsAsync(searchDto);

            // Map to DTOs
            var postReadDtos = new List<PostReadDto>();
            foreach (var post in posts)
            {
                var overallRating = await _ratingRepository.GetOverallCriteriaByPostId(post.Id);
                int totalComments = await _commentRepository.GetTotalCommentByPost(post.Id);
                double averageStars = CalculateAverageStars(overallRating);
                postReadDtos.Add(MapPostToReadDto(post, totalComments, averageStars));
            }

            // Create and return pagination result
            return new PaginationDto<PostReadDto>
            {
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize,
                TotalPages = totalPages,
                TotalItems = totalCount,
                Items = postReadDtos
            };
        }

        public async Task<IEnumerable<PostReadDto>> GetAllPostsAsync()
        {
            var posts = await _postRepository.GetAllAsync();
            if (posts == null || !posts.Any())
            {
                return Enumerable.Empty<PostReadDto>();
            }

            var postReadDtos = new List<PostReadDto>();
            foreach (var post in posts)
            {
                //var comments = await _commentService.GetCommentsByPostAsync(post.Id);
                var overallRating = await _ratingRepository.GetOverallCriteriaByPostId(post.Id);
                
                int totalComments =  await _commentRepository.GetTotalCommentByPost(post.Id);
                double averageStars = CalculateAverageStars(overallRating);

                postReadDtos.Add(MapPostToReadDto(post, totalComments, averageStars));
            }
            return postReadDtos;
        }

        public async Task<Post> UpdatePostAsync(PostUpdateDto postDto, string userId)
        {
            // Check if post exists and belongs to the user
            var post = await _postRepository.GetByIdAsync(postDto.Id);
            if (post == null)
            {
                throw new NotFoundException($"Post with ID {postDto.Id} not found.");
            }

            if (post.UserId != userId)
            {
                throw new AppException("You are not authorized to update this post.");
            }

            // Update basic properties if provided
            if (!string.IsNullOrWhiteSpace(postDto.Name))
                post.Name = postDto.Name;

            if (!string.IsNullOrWhiteSpace(postDto.Title))
                post.Title = postDto.Title;

            if (!string.IsNullOrWhiteSpace(postDto.SDT))
                post.SDT = postDto.SDT;

            if (!string.IsNullOrWhiteSpace(postDto.Description))
                post.Description = postDto.Description;

            if (postDto.Price.HasValue)
                post.Price = postDto.Price;

            if (!string.IsNullOrWhiteSpace(postDto.LinkFacebook))
                post.LinkFacebook = postDto.LinkFacebook;

            if (!string.IsNullOrWhiteSpace(postDto.LinkGoogleMap))
                post.LinkGoogleMap = postDto.LinkGoogleMap;

            // Update address if any address components are provided
            var addressParts = new List<string?>();
            bool hasAddressUpdate = false;

            if (!string.IsNullOrWhiteSpace(postDto.Street))
            {
                addressParts.Add(postDto.Street.Trim());
                hasAddressUpdate = true;
            }
            
            if (!string.IsNullOrWhiteSpace(postDto.Ward))
            {
                addressParts.Add(postDto.Ward.Trim());
                hasAddressUpdate = true;
            }
            
            if (!string.IsNullOrWhiteSpace(postDto.District))
            {
                addressParts.Add(postDto.District.Trim());
                hasAddressUpdate = true;
            }
            
            if (!string.IsNullOrWhiteSpace(postDto.City))
            {
                addressParts.Add(postDto.City.Trim());
                hasAddressUpdate = true;
            }

            if (hasAddressUpdate)
            {
                post.Address = string.Join(", ", addressParts.Where(s => !string.IsNullOrEmpty(s)));
            }

            // Update category if provided
            if (postDto.CategoryId.HasValue && postDto.CategoryId.Value > 0)
            {
                var category = await _postRepository.GetCategoryByIdAsync(postDto.CategoryId.Value);
                if (category == null)
                {
                    throw new NotFoundException("Invalid Category ID.");
                }
                post.CategoryId = postDto.CategoryId.Value;
            }

            // Update tags if provided
            if (postDto.TagIds != null && postDto.TagIds.Any())
            {
                var tags = await _postRepository.GetTagsByIdsAsync(postDto.TagIds);
                post.Tags = tags;
            }

            // Handle Logo Image - replace if new one is provided
            if (postDto.LogoImage != null)
            {
                post.Logo_Image = await _saveImageHelper.SaveFileAsync(postDto.LogoImage, "logos");
            }
            
            // Handle Background Image - replace if new one is provided
            if (postDto.BackgroundImage != null)
            {
                post.Background = await _saveImageHelper.SaveFileAsync(postDto.BackgroundImage, "backgrounds");
            }

            // Handle Other Images - replace all if new ones are provided
            if (postDto.OtherImages != null && postDto.OtherImages.Any())
            {
                // Clear existing images
                post.Images.Clear();
                
                // Add new images
                foreach (var imageFile in postDto.OtherImages)
                {
                    var imagePath = await _saveImageHelper.SaveFileAsync(imageFile, "posts");
                    post.Images.Add(new Image
                    {
                        ImagePath = imagePath,
                        CommentId = null,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow
                    });
                }
            }

            // Update opening hours if provided
            if (postDto.OpeningHours != null && postDto.OpeningHours.Any())
            {
                // Clear existing opening hours
                post.Opening_Hours.Clear();

                // Add new opening hours
                foreach (var ohDto in postDto.OpeningHours)
                {
                    if (string.IsNullOrWhiteSpace(ohDto.DayOfWeek) ||
                        ohDto.OpenHour < 0 || ohDto.OpenHour > 23 || ohDto.OpenMinute < 0 || ohDto.OpenMinute > 59 ||
                        ohDto.CloseHour < 0 || ohDto.CloseHour > 23 || ohDto.CloseMinute < 0 || ohDto.CloseMinute > 59)
                    {
                        continue;
                    }

                    post.Opening_Hours.Add(new OpeningHour
                    {
                        DayOfWeek = ohDto.DayOfWeek,
                        OpenHour = ohDto.OpenHour,
                        OpenMinute = ohDto.OpenMinute,
                        CloseHour = ohDto.CloseHour,
                        CloseMinute = ohDto.CloseMinute,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow
                    });
                }
            }

            // Update timestamp
            post.UpdatedDate = DateTime.UtcNow;
            post.UpdatedUser = Guid.Parse(userId);

            // Save changes
            return await _postRepository.UpdatePostAsync(post);
        }

        public async Task LockPostAsync(string userId, int status)
        {
            var postsList = await _postRepository.GetByUserIdAsync(userId);
            if (postsList != null)
            {
                foreach (var post in postsList)
                {
                    post.Status = status;
                }
                await _postRepository.UpdatePostAsync(postsList.ToList());
            }
        }

        public async Task<long?> GetPostIdByUserAsync(string userId)
        {
           return await _postRepository.GetIdPostByUser(userId);
        }
    }
}