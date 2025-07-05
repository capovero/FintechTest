using Microsoft.AspNetCore.Mvc;
using SpreadCalculator.Domain.Entities;
using SpreadCalculator.Domain.Interfaces;

namespace SpreadCalculator.API.Controllers;

    [ApiController]
    [Route("api/[controller]")] 
    public class SpreadController : ControllerBase
    {
        private readonly ISpreadRepository _repository;

        public SpreadController(ISpreadRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpreadResult>>> GetAll()
        {
            var results = await _repository.GetAllAsync();
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SpreadResult>> GetById(int id)
        {
            var result = await _repository.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<SpreadResult>> Create([FromBody] SpreadResult newSpread)
        {
            var created = await _repository.AddAsync(newSpread);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
    }