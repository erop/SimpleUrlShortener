using Microsoft.EntityFrameworkCore;
using SimpleUrlShortener.Core.Entities;

namespace SimpleUrlShortener.Core.Persistence;

public class DataContext : DbContext
{
    protected DataContext()
    {
    }

    public DataContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("utf8mb4_bin");
    }

    public virtual DbSet<ShortUrl> ShortUrls => Set<ShortUrl>();
}