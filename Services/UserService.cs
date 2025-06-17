using Click_Go.DTOs;
using Click_Go.Repositories;
using Click_Go.Repositories.Interfaces;
using Click_Go.Services.Interfaces;

namespace Click_Go.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        private readonly IOrderRepository _orderRepository;

        public UserService(IUserRepository userRepository, IPostRepository postRepository, IOrderRepository orderRepository )
        {
            _userRepository = userRepository;
            _postRepository = postRepository;
            _orderRepository = orderRepository;
        }

        public Task<UpdateProfileDto> GetProfileAsync(string userId)
        {
            return _userRepository.GetProfileAsync(userId);
        }

        public async Task<GetTotalDto> GetTotal(int? statusPost, DateTime? from, DateTime? to)
        {
            return new GetTotalDto
            {
                TotalUser = await _userRepository.GetTotalUser(),
                TotalPost = await _postRepository.GetTotalPostAsync(statusPost),
                TotalRevenue = await _orderRepository.GetTotalRevenue(from,to)
            };
        }

        public async Task UpdateAsync(UpdateProfileDto dto)
        {
            await _userRepository.UpdateAsync(dto);
        }
    }

}


