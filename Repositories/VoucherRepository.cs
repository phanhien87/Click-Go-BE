using Click_Go.Data;
using Click_Go.DTOs;
using Click_Go.Helper;
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


        async Task<List<Voucher>> IVoucherRepository.GetallByPostIdAsync(long id)
        {
            return await _context.Vouchers.Where(v => v.PostId == id).ToListAsync();
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
                PostId = dto.PostId,
                Status = 1
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
            existing.UsedCount = dto.UsedCount ?? 0;
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

        public async Task<long> UpdateUsedCountAsync(long idVoucher, bool action)
        {
            var vouncher = await _context.Vouchers.FindAsync(idVoucher);

            if (vouncher == null) throw new NotFoundException("Không tìm thấy vouncher nào!");

            vouncher.UsedCount = action ? ++vouncher.UsedCount : --vouncher.UsedCount;

            if (vouncher.UsedCount == vouncher.UsageLimit) vouncher.IsActive = false;
            else vouncher.IsActive = true;

                await _context.SaveChangesAsync();
            return vouncher.PostId;
        }
    }
}
