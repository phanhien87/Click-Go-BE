using Click_Go.DTOs;
using Click_Go.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Click_Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var result = await _authService.RegisterAsync(model);
            return result == "Success" ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var token = await _authService.LoginAsync(model);
            return token == null ? Unauthorized() : Ok(new { token });
        }
    }
}
