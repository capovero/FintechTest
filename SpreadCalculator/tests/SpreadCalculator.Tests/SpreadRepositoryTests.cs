using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpreadCalculator.Domain.Entities;
using SpreadCalculator.Infrastructure;
using SpreadCalculator.Infrastructure.Configurations;
using SpreadCalculator.Infrastructure.Repositories;
using Xunit;

namespace SpreadCalculator.Tests
{
    public class SpreadRepositoryTests
    {
        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAndGetSpreads_WorksCorrectly()
        {
            await using var ctx = CreateContext();
            var repo = new SpreadRepository(ctx);
            var spread = new SpreadResult
            {
                Timestamp = DateTime.UtcNow,
                NearPrice = 10m,
                FarPrice  = 12m
            };

            await repo.AddSpreadAsync(spread);
            var all = (await repo.GetSpreadsAsync()).ToList();

            Assert.Single(all);
            Assert.Equal(2m, all[0].Spread);
            Assert.Equal(spread.Timestamp, all[0].Timestamp, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task GetSpreads_ReturnsEmpty_WhenNoData()
        {
            await using var ctx = CreateContext();
            var repo = new SpreadRepository(ctx);

            var all = await repo.GetSpreadsAsync();

            Assert.Empty(all);
        }
    }
}