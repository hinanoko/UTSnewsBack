using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Models
{
    public class User
    {
        public int Id { get; set; } // 主键
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string Identity { get; set; }
    }
}
