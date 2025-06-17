using Click_Go.Data;
using Click_Go.DTOs;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Click_Go.Repositories
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly ApplicationDbContext _context;

        public VoucherRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Voucher> CreateAsync(VoucherProcessDto dto)
        {
            var voucher = new Voucher
            {
                Code = dto.Code,
                DiscountAmount = dto.DiscountAmount,
                DiscountPercentage = dto.DiscountPercentage,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive,
                UsageLimit = dto.UsageLimit,
                UsedCount = dto.UsedCount,
                PostId = dto.PostId
            };
            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();
            return voucher;
        }

        public async Task<Voucher?> UpdateAsync(long id, VoucherProcessDto dto)
        {
            var existing = await _context.Vouchers.FindAsync(id);
            if (existing == null) return null;

            existing.Code = dto.Code;
            existing.DiscountAmount = dto.DiscountAmount;
            existing.DiscountPercentage = dto.DiscountPercentage;
            existing.Description = dto.Description;
            existing.StartDate = dto.StartDate;
            existing.EndDate = dto.EndDate;
            existing.IsActive = dto.IsActive;
            existing.UsageLimit = dto.UsageLimit;
            existing.UsedCount = dto.UsedCount;
            existing.PostId = dto.PostId;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<Voucher?> GetByIdAsync(long id)
        {
            return await _context.Vouchers.FindAsync(id);
        }
        
        public async Task<Voucher?> GetByCodeAsync(string code)
        {
            return await _context.Vouchers.FirstOrDefaultAsync(x => x.Code == code);
        }
    }
}
