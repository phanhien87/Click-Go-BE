using Click_Go.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
