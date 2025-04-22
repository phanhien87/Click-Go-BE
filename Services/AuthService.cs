using Click_Go.DTOs;
using Click_Go.Models;
using Click_Go.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Click_Go.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Sai tên đăng nhập.");

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
                throw new UnauthorizedAccessException("Sai mật khẩu.");

            return await GenerateJwtTokenAsync(user);
        }

        public async Task<string> RegisterAsync(RegisterDto model)
        {
            var user = new ApplicationUser { UserName = model.Email.Split('@')[0], Email = model.Email, FullName = model.FullName };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "CUSTOMER");
                return "Success";
            }

            return string.Join(", ", result.Errors.Select(e => e.Description));
        }

        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            // Lấy danh sách các role của user (ví dụ: "CUSTOMER", "ADMIN")
            var userRoles = await _userManager.GetRolesAsync(user);

            // Tạo danh sách claims
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Thêm claim role vào token
            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Tạo key ký JWT
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            // Tạo token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            // Trả token về dưới dạng chuỗi
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }

}


