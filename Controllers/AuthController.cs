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
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _authService = authService;
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
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
            
            Response.OnStarting(() =>
            {
                foreach (var header in Response.Headers)
                {
                    Console.WriteLine($"Header: {header.Key} = {header.Value}");
                }
                return Task.CompletedTask;
            });
            
            return Challenge(properties, provider);
        }

        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            string frontendUrl = _configuration["FrontendUrl"] ?? "https://clickgo.dev";

            try
            {
                if (remoteError != null)
                    return Redirect($"{frontendUrl}/login?error={remoteError}");

                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                    return Redirect($"{frontendUrl}/login?error=Không thể lấy thông tin đăng nhập");

                var signInResult =
                    await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                        isPersistent: false);

                if (signInResult.Succeeded)
                {
                    var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                    var token = await _authService.GenerateJwtTokenAsync(user);
                    return Redirect($"{frontendUrl}/login?token={token}");
                }
                else
                {
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    var existingUser = await _userManager.FindByEmailAsync(email);

                    if (existingUser != null)
                    {
                        var addLoginResult = await _userManager.AddLoginAsync(existingUser, info);
                        if (addLoginResult.Succeeded)
                        {
                            var token = await _authService.GenerateJwtTokenAsync(existingUser);
                            return Redirect($"{frontendUrl}/login?token={token}");
                        }

                        return Redirect($"{frontendUrl}/login?error=Liên kết tài khoản thất bại");
                    }
                    else
                    {
                        var user = new ApplicationUser
                        {
                            UserName = email.Split('@')[0],
                            Email = email,
                            FullName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email.Split('@')[0]
                        };

                        var createResult = await _userManager.CreateAsync(user);

                        if (createResult.Succeeded)
                        {
                            await _userManager.AddLoginAsync(user, info);
                            await _userManager.AddToRoleAsync(user, "CUSTOMER");

                            var token = await _authService.GenerateJwtTokenAsync(user);
                            return Redirect($"{frontendUrl}/login?token={token}");
                        }
                        else
                        {
                            return Redirect($"{frontendUrl}/login?error=Tạo tài khoản thất bại");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OAuth ERROR] {ex.Message}\n{ex.StackTrace}");
                return Redirect($"{frontendUrl}/login?error=Server OAuth lỗi: {ex.Message}");
            }
        }
    }
}