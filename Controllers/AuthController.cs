using Click_Go.DTOs;
using Click_Go.Models;
using Click_Go.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Click_Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthController(IAuthService authService, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _signInManager = signInManager;
            _userManager = userManager;
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
            try
            {
                var token = await _authService.LoginAsync(model);
                return Ok(new { token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error." });
            }
        }

        [HttpGet("external-login")]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
                return BadRequest($"Error: {remoteError}");

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return BadRequest("Error loading external login information.");

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (signInResult.Succeeded)
            {
                // Tạo jwt
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                var token = await _authService.GenerateJwtTokenAsync(user);
                return Ok(new { token });
            }
            else
            {
                // Tk mới
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email == null)
                    return BadRequest("Email not available.");

                var user = new ApplicationUser
                {
                    UserName = email.Split('@')[0],
                    Email = email,
                    FullName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email.Split('@')[0]
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                await _userManager.AddLoginAsync(user, info);
                await _userManager.AddToRoleAsync(user, "CUSTOMER");

                var token = await _authService.GenerateJwtTokenAsync(user);
                return Ok(new { token });
            }
        }

    }
}
