using SpreadCalculator.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpreadCalculator.Domain.Interfaces
{
    public interface ISpreadRepository
    {
        Task<IEnumerable<SpreadResult>> GetSpreadsAsync();
        Task AddSpreadAsync(SpreadResult spread);
    }
}