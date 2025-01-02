using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public sealed class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt)
            : base(opt) { }

        public DbSet<Platform> Platforms { get; set; }
    }
}
