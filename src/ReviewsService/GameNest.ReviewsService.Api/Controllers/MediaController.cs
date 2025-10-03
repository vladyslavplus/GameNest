using GameNest.ReviewsService.Application.Commands.MediaCommands.CreateMedia;
using GameNest.ReviewsService.Application.Commands.MediaCommands.DeleteMedia;
using GameNest.ReviewsService.Application.Commands.MediaCommands.UpdateMediaUrl;
using GameNest.ReviewsService.Application.Queries.MediaQueries.GetMedia;
using GameNest.ReviewsService.Application.Queries.MediaQueries.GetMediaById;
using GameNest.ReviewsService.Domain.Entities.Parameters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.ReviewsService.Api.Controllers
{
    public class MediaController : BaseApiController
    {
        public MediaController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetMediaByIdQuery { MediaId = id }, ct);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] MediaParameters parameters, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetMediaQuery { Parameters = parameters }, ct);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMediaCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}/url")]
        public async Task<IActionResult> UpdateUrl(string id, [FromBody] UpdateMediaUrlCommand command, CancellationToken ct)
        {
            await _mediator.Send(command with { MediaId = id }, ct);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            await _mediator.Send(new DeleteMediaCommand { MediaId = id }, ct);
            return NoContent();
        }
    }
}