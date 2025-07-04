using Click_Go.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Click_Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            this._packageService = packageService;
        }

        [HttpGet("AllPackages")]
        [AllowAnonymous]
        public async Task<IActionResult> AllPackages()
        {
            return Ok(await _packageService.GetAllPackagesAsync());
        }
    }
}
