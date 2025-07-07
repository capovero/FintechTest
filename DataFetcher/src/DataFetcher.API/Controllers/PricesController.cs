using Microsoft.AspNetCore.Mvc;
using DataFetcher.API.Services;
using System.Threading.Tasks;
using DataFetcher.Application.Jobs;

namespace DataFetcher.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PricesController : ControllerBase
    {
        private readonly FetchPricesJob _fetchPricesJob;

        public PricesController(FetchPricesJob fetchPricesJob)
        {
            _fetchPricesJob = fetchPricesJob;
        }

        [HttpGet("fetch")]
        public async Task<IActionResult> Fetch()
        {
            await _fetchPricesJob.ExecuteAsync();
            return Ok("Цены успешно отправлены в очередь.");
        }
    }
}