using Click_Go.Data;
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
    }
}
