using GameNest.AggregatorService.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.AggregatorService.Controllers
{
    [ApiController]
    [Route("api/aggregated/orders")]
    public class OrderAggregatorController : ControllerBase
    {
        private readonly OrderAggregatorService _orderAggregator;

        public OrderAggregatorController(OrderAggregatorService orderAggregator)
        {
            _orderAggregator = orderAggregator;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _orderAggregator.GetAggregatedOrderAsync(id, ct);
            return result is null ? NotFound() : Ok(result);
        }
    }
}
