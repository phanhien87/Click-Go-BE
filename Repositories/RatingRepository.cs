using Click_Go.Data;
using Click_Go.DTOs;
using Click_Go.Enum;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Click_Go.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly ApplicationDbContext _context;
        public RatingRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Rating rating)
        {
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();
        }

        public async Task<double?> GetOverallByCmtId(long id)
        {
           var rt =  await _context.Ratings.FirstOrDefaultAsync(r => r.CommentId == id);
            if (rt == null) return null;
            return rt.Overall;
        }

        public async Task<OverallCriteriaDto> GetOverallCriteriaByPostId(long id)
        {
           var details = await _context.RatingDetails.Include(r => r.Rating).Where(p => p.Rating.PostId == id).GroupBy(c =>c.Criteria).Select(g => new
            {
                CriteriaName = g.Key,
                AverageScore = g.Average(s =>s.Score),
            }).ToListAsync();

            return new OverallCriteriaDto
            {
                Quality = details.FirstOrDefault(x => x.CriteriaName == Criteria.Quality)?.AverageScore ?? 0,
                Location = details.FirstOrDefault(x => x.CriteriaName == Criteria.Location)?.AverageScore ?? 0,
                Space = details.FirstOrDefault(x => x.CriteriaName == Criteria.Space)?.AverageScore ?? 0,
                Price = details.FirstOrDefault(x => x.CriteriaName == Criteria.Price)?.AverageScore ?? 0,
                Service = details.FirstOrDefault(x => x.CriteriaName == Criteria.Service)?.AverageScore ?? 0,

            };

           
        }
    }
}
