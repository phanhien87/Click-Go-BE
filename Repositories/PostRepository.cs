﻿using Click_Go.Data;
using Click_Go.DTOs;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Click_Go.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Post> CreateAsync(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Post> GetByIdAsync(long id)
        {
            return await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.Images)
                .Include(p => p.Opening_Hours)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Post>> GetByUserIdAsync(string userId)
        {
            return await _context.Posts
                .Where(p => p.UserId == userId)
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.Images)
                .Include(p => p.Opening_Hours)
                .Include(p => p.Tags)
                .ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(long categoryId)
        {
            return await _context.Categories.FindAsync(categoryId);
        }

        public async Task<IEnumerable<Post>> SearchPostsAsync(PostSearchDto searchDto)
        {
            // Start with an IQueryable that includes necessary related entities
            var query = _context.Posts
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.Images)
                .Include(p => p.Opening_Hours)
                .Include(p => p.Tags)
                .AsQueryable();

            // Filter by post name if provided
            if (!string.IsNullOrWhiteSpace(searchDto.PostName))
            {
                var postNameLower = searchDto.PostName.ToLower().Trim();
                query = query.Where(p => p.Name != null && p.Name.ToLower().Contains(postNameLower));
            }

            // Process address components
            var addressComponents = new List<string>();
            if (!string.IsNullOrWhiteSpace(searchDto.District))
            {
                addressComponents.Add(searchDto.District.ToLower().Trim());
            }
            if (!string.IsNullOrWhiteSpace(searchDto.Ward))
            {
                addressComponents.Add(searchDto.Ward.ToLower().Trim());
            }
            if (!string.IsNullOrWhiteSpace(searchDto.City))
            {
                addressComponents.Add(searchDto.City.ToLower().Trim());
            }

            // Apply address filtering if components exist
            if (addressComponents.Any())
            {
                query = query.Where(p => p.Address != null && 
                                         addressComponents.All(comp => p.Address.ToLower().Contains(comp)));
            }

            // Fix tag filtering to use a technique EF Core can translate to SQL
            if (searchDto.TagNames != null && searchDto.TagNames.Any(t => !string.IsNullOrWhiteSpace(t)))
            {
                var lowerCaseTagNames = searchDto.TagNames
                    .Where(tn => !string.IsNullOrWhiteSpace(tn))
                    .Select(tn => tn.ToLower().Trim())
                    .Distinct()
                    .ToList();

                // Materialize posts to perform tag filtering in memory
                var postsWithIncludes = await query.ToListAsync();

                // Filter in memory for tags (ALL logic)
                return postsWithIncludes.Where(p => 
                    p.Tags != null && 
                    lowerCaseTagNames.All(tagName => 
                        p.Tags.Any(t => t.Name.ToLower() == tagName)));
            }

            // Apply price range filters
            if (searchDto.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price.HasValue && p.Price >= searchDto.MinPrice.Value);
            }

            if (searchDto.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price.HasValue && p.Price <= searchDto.MaxPrice.Value);
            }
            
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.Opening_Hours)
                .Include(p => p.Images)
                .Include(p => p.Tags)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<UserPackage> GetUserPackageByUserIdAsync(string userId)
        {
            return await _context.UserPackages
                                 .FirstOrDefaultAsync(up => up.UserId == userId);
        }

        public async Task UpdatePostAsync(List<Post> post)
        {
            _context.Posts.UpdateRange(post);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalPostAsync(int? status)
        {
            if (status.HasValue) return await _context.Posts.CountAsync(p => p.Status == status);
            else return await _context.Posts.CountAsync();
        }

        public async Task<List<Tag>> GetTagsByIdsAsync(List<long> tagIds)
        {
            return await _context.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync();
        }
    }
}
