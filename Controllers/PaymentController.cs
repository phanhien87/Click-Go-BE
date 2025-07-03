using Click_Go.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using System.Security.Claims;

namespace Click_Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PayOSService _payOSService;

        public PaymentController(PayOSService payOSService)
        {
            _payOSService = payOSService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var url = await _payOSService.CreatePaymentLink(request.PackageId, request.ReturnUrl, request.CancelUrl,
                userId);

            return Ok(url);
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> PayOSWebhook([FromBody] WebhookType request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var status = await _payOSService.ConfirmPayment(request);

                if (status)
                {
                    return Ok(new { success = true, message = "Thanh toán thành công" });
                }

                return Ok(new { success = false, message = "Thanh toán thất bại" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi trong webhook: " + ex.ToString());


                return StatusCode(500, new { error = ex.Message, detail = ex.ToString() });
            }
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
