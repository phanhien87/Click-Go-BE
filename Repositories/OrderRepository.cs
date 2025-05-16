using Click_Go.Data;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Click_Go.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public Task DeleteAsync(Order order)
        {
            throw new NotImplementedException();
        }

        public async Task<Order> GetByCodeAsync(long code)
        {
            return await _context.Orders.Include(p => p.Package).FirstOrDefaultAsync(o => o.OrderCode == code.ToString());
        }

        public async Task UpdateAsync(Order order)
        {
             _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
