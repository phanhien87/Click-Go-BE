using Click_Go.Data;
using Click_Go.DTOs;
using Click_Go.Models;
using Click_Go.Helper;
using Click_Go.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Click_Go.Services
{
    public class PostService : IPostService
    {
        private readonly SaveImage _saveImageHelper;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, SaveImage saveImageHelper)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _saveImageHelper = saveImageHelper;
        }

        public async Task<Post> CreatePostAsync(PostCreateDto postDto, string userId)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(postDto.Name) || postDto.CategoryId <= 0)
            {
                throw new ArgumentException("Post name and category ID are required.");
            }

            var category = await _context.Categories.FindAsync(postDto.CategoryId);
            if (category == null)
            {
                throw new ArgumentException("Invalid Category ID.");
            }

            var post = new Post
            {
                Name = postDto.Name,
                Title = postDto.Title,
                SDT = postDto.SDT,
                Address = postDto.Address,
                Description = postDto.Description,
                CategoryId = postDto.CategoryId,
                UserId = userId, 
                Images = new List<Image>(),
                Opening_Hours = new List<OpeningHour>(), 
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            // Handle Logo Image Upload
            if (postDto.LogoImage != null)
            {
                post.Logo_Image = await _saveImageHelper.SaveFileAsync(postDto.LogoImage, "logos");
            }

            // Handle Background Image Upload
            if (postDto.BackgroundImage != null)
            {
                post.Background = await _saveImageHelper.SaveFileAsync(postDto.BackgroundImage, "backgrounds");
            }

            // Handle Other Images Upload
            if (postDto.OtherImages != null && postDto.OtherImages.Any())
            {
                foreach (var imageFile in postDto.OtherImages)
                {
                    var imagePath = await _saveImageHelper.SaveFileAsync(imageFile, "posts");
                    post.Images.Add(new Image
                    {
                        ImagePath = imagePath,
                        CommentId = null, // Image is for the post, not a comment
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow
                    });
                }
            }

            // Handle Opening Hours
            if (postDto.OpeningHours != null && postDto.OpeningHours.Any())
            {
                foreach (var ohDto in postDto.OpeningHours)
                {
                    // Basic validation for opening hours
                    if (string.IsNullOrWhiteSpace(ohDto.DayOfWeek) ||
                        ohDto.OpenHour < 0 || ohDto.OpenHour > 23 || ohDto.OpenMinute < 0 || ohDto.OpenMinute > 59 ||
                        ohDto.CloseHour < 0 || ohDto.CloseHour > 23 || ohDto.CloseMinute < 0 || ohDto.CloseMinute > 59)
                    {
                        // Optionally throw an exception or handle invalid data
                        // For now, we'll just skip invalid entries
                        continue;
                    }

                    post.Opening_Hours.Add(new OpeningHour
                    {
                        DayOfWeek = ohDto.DayOfWeek,
                        OpenHour = ohDto.OpenHour,
                        OpenMinute = ohDto.OpenMinute,
                        CloseHour = ohDto.CloseHour,
                        CloseMinute = ohDto.CloseMinute,
                        // PostId will be set automatically by EF Core relationship
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow
                    });
                }
            }

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return post;
        }

        public async Task<Post> GetPostByIdAsync(long id)
        {
            // Use Include to eagerly load related data needed for the DTO
            var post = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.Images)
                .Include(p => p.Opening_Hours)
                .FirstOrDefaultAsync(p => p.Id == id);

            return post; // Controller will handle null check (NotFound)
        }

        public async Task<IEnumerable<Post>> GetPostsByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                // Or throw an ArgumentNullException, depending on desired behavior
                return Enumerable.Empty<Post>(); 
            }

            // Use Include to eagerly load related data needed for the DTO
            var posts = await _context.Posts
                .Where(p => p.UserId == userId)
                .Include(p => p.Category)
                .Include(p => p.User) // Technically redundant if filtering by UserId, but good practice
                .Include(p => p.Images)
                .Include(p => p.Opening_Hours)
                .ToListAsync(); // Fetch the list of posts

            return posts;
        }

    }
} 