using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Click_Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "CUSTOMER")]
    public class CustomerController : ControllerBase
    {
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            return Ok("Welcome CUSTOMER - This is the Customer Profile.");
        }
    }
}
