using Click_Go.Models;

namespace Click_Go.Repositories.Interfaces
{
    public interface IPackageRepository
    {
        Task<List<Package>> GetAllPackagesAsync();
        Task<Package> GetPackageByIdAsync(long id);
    }
}
