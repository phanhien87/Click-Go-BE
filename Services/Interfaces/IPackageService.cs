using Click_Go.Models;

namespace Click_Go.Services.Interfaces
{
    public interface IPackageService
    {
        Task<List<Package>> GetAllPackagesAsync();
    }
}
