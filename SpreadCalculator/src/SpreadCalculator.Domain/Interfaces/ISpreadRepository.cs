using SpreadCalculator.Domain.Entities;

namespace SpreadCalculator.Domain.Interfaces
{
    public interface ISpreadRepository
    {
        Task<List<SpreadResult>> GetAllAsync();
        Task<SpreadResult?> GetByIdAsync(int id);
        Task<SpreadResult> AddAsync(SpreadResult spread);
    }
}