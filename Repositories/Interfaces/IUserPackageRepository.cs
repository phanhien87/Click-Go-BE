using Click_Go.Models;

namespace Click_Go.Repositories.Interfaces
{
    public interface IUserPackageRepository
    {
        Task AddAsync(UserPackage userPackage);
        Task<List<UserPackage>> GetAllAsync();
    }
}
