using Microsoft.EntityFrameworkCore;
using SpreadCalculator.Domain.Entities;
using SpreadCalculator.Infrastructure.Configurations;

namespace SpreadCalculator.Infrastructure.Configurations
{
    public class AppDbContext : DbContext
    {
        public DbSet<SpreadResult> SpreadResults { get; set; }
        public DbSet<FuturePrice> FuturePrices { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SpreadResultConfiguration());
            modelBuilder.ApplyConfiguration(new FuturePriceConfiguration());
        }
    }
}