using Click_Go.DTOs;
using Click_Go.Models;
using Click_Go.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Click_Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "CUSTOMER")]
    public class CustomerVoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public CustomerVoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoucher([FromBody] VoucherProcessDto dto)
        {
            var created = await _voucherService.CreateVoucherAsync(dto);
            return CreatedAtAction(nameof(GetVoucher), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVoucher(long id, [FromBody] VoucherProcessDto dto)
        {
            var updated = await _voucherService.UpdateVoucherAsync(id, dto);
            if (updated == null)
                return NotFound();

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


    }
}
