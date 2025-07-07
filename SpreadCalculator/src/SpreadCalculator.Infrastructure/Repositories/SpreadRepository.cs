using Microsoft.EntityFrameworkCore;
using SpreadCalculator.Domain.Entities;
using SpreadCalculator.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpreadCalculator.Infrastructure.Configurations;

namespace SpreadCalculator.Infrastructure.Repositories
{
    public class SpreadRepository : ISpreadRepository
    {
        private readonly AppDbContext _dbContext;

        public SpreadRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<SpreadResult>> GetSpreadsAsync()
        {
            return await _dbContext.SpreadResults.ToListAsync();
            
        }

        public async Task AddSpreadAsync(SpreadResult spread)
        {
            await _dbContext.SpreadResults.AddAsync(spread);
            await _dbContext.SaveChangesAsync();
        }
    }
}