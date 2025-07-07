using System.Threading.Tasks;

namespace DataFetcher.Application.Interfaces
{
    public interface IFuturesFetcherService
    {
        Task<decimal?> GetPriceAsync(string contractCode);
    }
}