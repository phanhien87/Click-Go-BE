using Click_Go.DTOs;
using Click_Go.Repositories.Interfaces;
using Click_Go.Services.Interfaces;

namespace Click_Go.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository orderRepository)
        {
            _userRepository = orderRepository;
        }

        public Task<UpdateProfileDto> GetProfileAsync(string userId)
        {
            return _userRepository.GetProfileAsync(userId);
        }

        public async Task UpdateAsync(UpdateProfileDto dto)
        {
            await _userRepository.UpdateAsync(dto);
        }
    }

}


