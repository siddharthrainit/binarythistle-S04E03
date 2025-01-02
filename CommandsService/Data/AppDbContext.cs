using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public required DbSet<Platform> Platforms { get; set; }
    public required DbSet<Command> Commands { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (modelBuilder is null)
        {
            throw new ArgumentNullException(nameof(modelBuilder));
        }

        _ = modelBuilder
            .Entity<Platform>()
            .HasMany(p => p.Commands)
            .WithOne(p => p.Platform)
            .HasForeignKey(p => p.PlatformId);

        _ = modelBuilder
            .Entity<Command>()
            .HasOne(p => p.Platform)
            .WithMany(p => p.Commands)
            .HasForeignKey(p => p.PlatformId);
    }
}
