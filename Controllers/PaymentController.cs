using Click_Go.DTOs;
using Click_Go.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Click_Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "CUSTOMER")]
    public class PaymentController : ControllerBase
    {
        private readonly PayOSService _payOSService;

        public PaymentController(PayOSService payOSService)
        {
            _payOSService = payOSService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
         
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var url = await _payOSService.CreatePaymentLink(
                request.Amount, request.Description,request.ReturnUrl  ,request.CancelUrl, request.level
            );

            return Ok( url );
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> PayOSWebhook([FromBody] CreatePaymentLinkDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var status = await _payOSService.ConfirmPayment(request, userId);
            if (status)
            {
                // Trả về JSON báo thành công
                return Ok(new { success = true, message = "Thanh toán thành công" });
            }

            return Ok(new { success = false, message = "Thanh toán thất bại" });
        }

    }

    public class CreatePaymentRequest
    {
        public int Amount { get; set; }
        public string Description { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
        public int? level { get; set; }
    }
}

