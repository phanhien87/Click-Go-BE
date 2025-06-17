using Click_Go.DTOs;
using Click_Go.Models;

namespace Click_Go.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<UpdateProfileDto> GetProfileAsync(string? id);
        Task UpdateAsync(UpdateProfileDto dto);
        Task<int> GetTotalUser();
    }
}
