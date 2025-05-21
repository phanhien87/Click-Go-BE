using Click_Go.DTOs;
using Click_Go.Helper;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Click_Go.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Click_Go.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IPostRepository _postRepository; 

        public WishlistService(IWishlistRepository wishlistRepository, IPostRepository postRepository)
        {   
            _wishlistRepository = wishlistRepository;
            _postRepository = postRepository;
        }

        public async Task<bool> ToggleWishlistAsync(string userId, long postId)
        {   
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
            {   
                throw new NotFoundException($"Post with ID {postId} not found.");
            }

            var wishlistItem = await _wishlistRepository.GetAsync(userId, postId);

            if (wishlistItem == null)
            {   
                // Add to wishlist
                var newWishlistItem = new Wishlist
                {   
                    UserId = userId,
                    PostId = postId,
                    DateAdded = DateTime.UtcNow
                };
                await _wishlistRepository.AddAsync(newWishlistItem);
                return true; // Added
            }
            else
            {   
                // Remove from wishlist
                await _wishlistRepository.RemoveAsync(wishlistItem);
                return false; // Removed
            }
        }

        public async Task<IEnumerable<PostReadDto>> GetUserWishlistAsync(string userId)
        {   
            if (string.IsNullOrEmpty(userId))
            {   
                throw new AppException("User ID cannot be null or empty.");
            }

            var posts = await _wishlistRepository.GetUserWishlistPostsAsync(userId);
            
            // This mapping logic is similar to PostController.MapPostToReadDto
            // Consider centralizing mapping logic (e.g., using AutoMapper or a helper class)
            return posts.Select(p => new PostReadDto
            {   
                Id = p.Id,
                Name = p.Name,
                Title = p.Title,
                Logo_Image = p.Logo_Image,
                Background = p.Background,
                SDT = p.SDT,
                Address = p.Address,
                Description = p.Description,
                CreatedDate = p.CreatedDate,
                UpdatedDate = p.UpdatedDate,
                Category = p.Category != null ? new CategoryDto { Id = p.Category.Id, Name = p.Category.Name } : null,
                User = p.User != null ? new UserDto { Id = p.User.Id, UserName = p.User.UserName, FullName = p.User.FullName } : null,
                OpeningHours = p.Opening_Hours?.Select(oh => new OpeningHourDto
                {   
                    DayOfWeek = oh.DayOfWeek,
                    OpenHour = oh.OpenHour ?? 0,
                    OpenMinute = oh.OpenMinute ?? 0,
                    CloseHour = oh.CloseHour ?? 0,
                    CloseMinute = oh.CloseMinute ?? 0
                }).ToList() ?? new List<OpeningHourDto>(),
                Images = p.Images?.Select(img => new ImageDto { Id = img.Id, ImagePath = img.ImagePath }).ToList() ?? new List<ImageDto>()
            }).ToList();
        }
    }
} 