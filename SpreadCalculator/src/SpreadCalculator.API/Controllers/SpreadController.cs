using Microsoft.AspNetCore.Mvc;
using SpreadCalculator.Domain.Interfaces;

namespace SpreadCalculator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpreadController : ControllerBase
    {
        private readonly ISpreadRepository _spreadRepository;

        public SpreadController(ISpreadRepository spreadRepository)
        {
            _spreadRepository = spreadRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var spreads = await _spreadRepository.GetSpreadsAsync();
            return Ok(spreads);
        }
    }
}