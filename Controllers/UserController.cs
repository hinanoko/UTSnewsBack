using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Name) ||
                string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest("Invalid user data.");
            }

            _context.Users.Add(user); // 添加用户
            await _context.SaveChangesAsync(); // 保存到数据库

            return Ok($"User {user.Name} registered successfully with email {user.Email}.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginUser)
        {
            if (loginUser == null || string.IsNullOrWhiteSpace(loginUser.Email) ||
                string.IsNullOrWhiteSpace(loginUser.Password))
            {
                return BadRequest("Invalid login data.");
            }

            // 验证用户
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginUser.Email && u.Password == loginUser.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // 登录成功，返回用户对象和成功消息
            return Ok(new
            {
                message = "Login successful.",
                user = user
            });
        }
    }
}
