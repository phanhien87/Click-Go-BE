using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Click_Go.Services.Interfaces;

namespace Click_Go.Services
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;

        public PackageService(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

        public async Task<List<Package>> GetAllPackagesAsync()
        {
           return await _packageRepository.GetAllPackagesAsync();
        }
    }
}
