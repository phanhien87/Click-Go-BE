using Click_Go.DTOs;
using Click_Go.Enum;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Click_Go.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Click_Go.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<OrderCodeDto>> GetOrdersAsync()
        {
            return await _orderRepository.GetOrdersAsync();
        }

    }

}


