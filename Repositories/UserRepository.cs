using Click_Go.Data;
using Click_Go.DTOs;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Click_Go.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UpdateProfileDto> GetProfileAsync(string? id)
        {
            var dto = new UpdateProfileDto();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                dto.FullName = user.FullName;
                dto.Address = user.Address;
                dto.PhoneNumber = user.PhoneNumber;
                dto.Email = user.Email;
            }

            return dto;

        }

        public async Task UpdateAsync(UpdateProfileDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user != null)
            {
                user.FullName = dto.FullName;
                user.Address = dto.Address;
                user.PhoneNumber = dto.PhoneNumber;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
