using Click_Go.Data;
using Click_Go.DTOs;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public async Task<Order> GetOrderListByUserIdAndPackageId(string userId, long packageId)
        {
            return await _context.Orders
                                        .Where(o => o.UserId == userId && o.PackageId == packageId)
                                        .OrderByDescending(o => o.CreatedDate) 
                                        .FirstOrDefaultAsync();
        }

        public async Task<List<OrderCodeDto>> GetOrdersAsync()
        {
            var orders = await _context.Orders.Include(p => p.Package).ToListAsync();
            return orders.Select(order => new OrderCodeDto
            {
                OrderCode = order.OrderCode,
                PackageId = order.Package.Id,
                transactionDateTime = order.transactionDateTime,
                counterAccountBankName = order.counterAccountBankName,
                counterAccountName = order.counterAccountName,
                counterAccountNumber = order.counterAccountNumber,
                Amount = order.Amount,
                Status = order.Status,
                UserId = order.UserId,
                TransactionId = order.TransactionId,
                Package = order.Package,
            }).ToList();
        }

        public async Task<long?> GetTotalRevenue(DateTime? from, DateTime? to)
        {
            IQueryable<Order> query = _context.Orders.Where(o => o.Status == Enum.OrderStatus.Paid);
            if(from.HasValue || to.HasValue)
            {
                if (from.HasValue)
                {

                    query = query.Where(o => o.CreatedDate >= from.Value);
                }
                if (to.HasValue)
                {

                    query = query.Where(o => o.CreatedDate <= to.Value);
                }
                return await query.SumAsync(o => o.Amount);
            }
        
            else  return await query.SumAsync(o => o.Amount);
        }

        public async Task UpdateAsync(Order order)
        {
             _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
