using Microsoft.EntityFrameworkCore;
using SpreadCalculator.Domain.Entities;

namespace SpreadCalculator.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<FuturePrice> FuturePrices { get; set; }
    public DbSet<SpreadResult> SpreadResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FuturePriceConfiguration());
        modelBuilder.ApplyConfiguration(new SpreadResultConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}