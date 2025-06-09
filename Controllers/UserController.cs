using System.Security.Claims;
using System.Threading.Tasks;
using Click_Go.Data;
using Click_Go.DTOs;
using Click_Go.Models;
using Click_Go.Services.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Click_Go.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(Enum.Application_Role.ADMIN))]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPostService _postService;
        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IPostService postService)
        {
            _context = context;
            _userManager = userManager;
            _postService = postService;
        }

        // GET: api/User
        [HttpGet(Name = "GetAllUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }

        // GET: api/User/{id}
        [HttpGet("{id}", Name = "GetUserById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ApplicationUser> GetUserById(string id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }


        [HttpPost("Lock-Unlock/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LockUnlock(string id)
        {
            var objFromDb = _context.Users.Find(id);
            if (objFromDb == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy người dùng" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                // Unlock  
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                // Lock
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            _context.Users.Update(objFromDb);
            _context.SaveChanges();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _postService.UpdatePostAsync(userId);

            return Ok(new{ success = true, message = "Thao tác thành công", lockoutEnd = objFromDb.LockoutEnd });
        }
    }
}
