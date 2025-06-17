using Click_Go.DTOs;
using Click_Go.Models;

namespace Click_Go.Services.Interfaces
{
    public interface IOrderService
    {
        public Task<List<OrderCodeDto>> GetOrdersAsync();
    }
}
