using Click_Go.DTOs;
using Click_Go.Models; 
using Click_Go.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; 
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic; 

namespace Click_Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService postService)
        {
            _orderService = postService;
        }

        [HttpGet]
        [Authorize(Roles = nameof(Enum.Application_Role.ADMIN))]
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetOrder()
        {
            var orders = await _orderService.GetOrdersAsync();
            return Ok(orders);
        }
    }
}
