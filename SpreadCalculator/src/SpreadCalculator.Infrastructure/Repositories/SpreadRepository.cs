using Microsoft.EntityFrameworkCore;
using SpreadCalculator.Domain.Entities;
using SpreadCalculator.Domain.Interfaces;


namespace SpreadCalculator.Infrastructure.Repositories
{
    public class SpreadRepository : ISpreadRepository
    {
        private readonly AppDbContext _context;

        public SpreadRepository(AppDbContext context) => _context = context;

        public async Task<List<SpreadResult>> GetAllAsync() =>
            await _context.SpreadResults.ToListAsync();

        public async Task<SpreadResult?> GetByIdAsync(int id) =>
            await _context.SpreadResults.FindAsync(id);

        public async Task<SpreadResult> AddAsync(SpreadResult spread)
        {
            var entry = await _context.SpreadResults.AddAsync(spread);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }
    }
}