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

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                return Conflict(new { message = "Email is already registered." });
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

        [HttpPut("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.NewPassword))
            {
                return BadRequest("Invalid data.");
            }

            // 查找用户
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // 更新密码
            user.Password = model.NewPassword;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password updated successfully." });
        }

        [HttpGet("getbyname/{name}")]
        public async Task<IActionResult> GetUsersByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Name cannot be empty.");
            }

            // 查找所有名称匹配的用户
            var users = await _context.Users.Where(u => u.Name == name).ToListAsync();

            if (users == null || !users.Any())
            {
                return NotFound(new { message = $"No users found with the name '{name}'." });
            }

            // 返回匹配的所有用户信息
            return Ok(users);
        }

    }

    public class ChangePasswordModel
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }

}
