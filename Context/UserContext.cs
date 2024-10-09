using System.Collections.Generic;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet 表示数据库中的用户表
        public DbSet<User> Users { get; set; }

        public DbSet<LostAndFoundModels> LostAndFoundItems { get; set; }

        public DbSet<FeedbackModels> Feedbacks { get; set; } // Feedback table
    }
}
