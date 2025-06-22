using Click_Go.DTOs;
using Click_Go.Helper;
using Click_Go.Hubs;
using Click_Go.Models;
using Click_Go.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Click_Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "CUSTOMER")]
    public class CustomerVoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        private readonly IHubContext<VoucherHub> _hubContext;
        public CustomerVoucherController(IVoucherService voucherService, IHubContext<VoucherHub> hubContext)
        {
            _voucherService = voucherService;
            _hubContext = hubContext;   
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoucher([FromBody] VoucherProcessDto dto)
        {
            var created = await _voucherService.CreateVoucherAsync(dto);
            await _hubContext.Clients.All.SendAsync("VoucherUpdated", new { created.PostId });
            return CreatedAtAction(nameof(GetVoucher), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVoucher(long id, [FromBody] VoucherProcessDto dto)
        {
            var updated = await _voucherService.UpdateVoucherAsync(id, dto);
            if (updated == null)
                return NotFound();
            await _hubContext.Clients.All.SendAsync("VoucherUpdated", new {updated.PostId});
            return Ok(updated);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoucher(long id)
        {
            var voucher = await _voucherService.GetVoucherByIdAsync(id);
            if (voucher == null)
                return NotFound();

            return Ok(voucher);
        }
        
        [HttpGet("GetByCode/{code}")]
        public async Task<IActionResult> GetVoucherByCode(string code)
        {
            var voucher = await _voucherService.GetVoucherByCodeAsync(code);
            if (voucher == null)
                return NotFound();

            return Ok(voucher);
        }

        [HttpGet("GetAllVoucher/{idPost}")]
        public async Task<IActionResult> GetAllVoucherByPostId(long idPost)
        {
            var listVoucher = await _voucherService.GetAllVoucherByPostIdAsync(idPost);
            if (listVoucher == null) throw new NotFoundException("Not Found");
            return Ok(listVoucher);
        }

        [HttpPut("{idVoucher}/used-count")]
        public async Task<IActionResult> UpdateUsedCount(long idVoucher, [FromBody] bool action)
        {
            var postId = await _voucherService.UpdateUsedCountAsync(idVoucher, action);
            await _hubContext.Clients.All.SendAsync("VoucherUpdated", new { postId });
            return NoContent();
        }

    }
}
