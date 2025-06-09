using Click_Go.DTOs;
using Click_Go.Models;

namespace Click_Go.Services.Interfaces
{
    public interface IUserService
    {
        Task<UpdateProfileDto> GetProfileAsync(string userId);
        Task UpdateAsync(UpdateProfileDto dto);
    }
}
