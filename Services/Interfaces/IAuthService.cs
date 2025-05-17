using Click_Go.DTOs;
using Click_Go.Models;

namespace Click_Go.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto model);
        Task<string> LoginAsync(LoginDto model);
        Task<string> GenerateJwtTokenAsync(ApplicationUser user);
    }
}
