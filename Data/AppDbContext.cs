using Microsoft.EntityFrameworkCore;
using ApiRestEf.Models;

namespace ApiRestEf.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
        public DbSet<Book> Books { get; set; }
        public DbSet<StudentInfo> StudentInfos { get; set; }
    }
}