using Microsoft.EntityFrameworkCore;

namespace FileShare.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<CustomFile> Files { get; set; } = null!;
        public DatabaseContext(DbContextOptions options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
