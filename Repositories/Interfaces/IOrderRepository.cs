using Click_Go.Models;

namespace Click_Go.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task DeleteAsync(Order order);
        Task<Order> GetByCodeAsync(long code);
        Task UpdateAsync(Order order);
        Task<Order> GetOrderListByUserIdAndPackageId(string id, long packageId);
    }
}
