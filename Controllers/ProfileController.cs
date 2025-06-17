using Click_Go.Data;
using Click_Go.DTOs;
using Click_Go.Models;
using Click_Go.Services.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Click_Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(Enum.Application_Role.CUSTOMER))]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public ProfileController(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet(Name = "MyProfile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfileAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound(new { success = false, message = "User not found" });
            }
            
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            return Ok(await _userService.GetProfileAsync(userId));
        }

        [HttpPut(Name = "UpdateProfile")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateProfileDto dto)
        {
            var users = _context.Users.FirstOrDefault(u => u.Email == dto.Email);

            if (users == null)
            {
                return NotFound(new { success = false, message = "User not found" });
            }

            await _userService.UpdateAsync(dto);
            return NoContent();
        }

        
    }
}
