using GameNest.AggregatorService.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.AggregatorService.Controllers
{
    [ApiController]
    [Route("api/aggregated/games")]
    public class GameAggregatorController : ControllerBase
    {
        private readonly GameAggregatorService _aggregatorService;

        public GameAggregatorController(GameAggregatorService aggregatorService)
        {
            _aggregatorService = aggregatorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _aggregatorService.GetAllAggregatedGamesAsync(ct);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _aggregatorService.GetAggregatedGameByIdAsync(id, ct);
            return result is null ? NotFound() : Ok(result);
        }
    }
}
