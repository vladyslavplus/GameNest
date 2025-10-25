using GameNest.ReviewsService.Application.Commands.MediaCommands.CreateMedia;
using GameNest.ReviewsService.Application.Commands.MediaCommands.DeleteMedia;
using GameNest.ReviewsService.Application.Commands.MediaCommands.UpdateMediaUrl;
using GameNest.ReviewsService.Application.Queries.MediaQueries.GetMedia;
using GameNest.ReviewsService.Application.Queries.MediaQueries.GetMediaById;
using GameNest.ReviewsService.Domain.Entities.Parameters;
using GameNest.ServiceDefaults.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.ReviewsService.Api.Controllers
{
    [Authorize]
    public class MediaController : BaseApiController
    {
        public MediaController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetMediaByIdQuery { MediaId = id }, ct);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] MediaParameters parameters, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetMediaQuery { Parameters = parameters }, ct);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMediaCommand command, CancellationToken ct)
        {
            var commandWithUser = command with { CustomerId = User.GetUserId() };
            var result = await _mediator.Send(commandWithUser, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}/url")]
        public async Task<IActionResult> UpdateUrl(string id, [FromBody] UpdateMediaUrlCommand command, CancellationToken ct)
        {
            var commandWithUser = command with { MediaId = id, RequesterId = User.GetUserId() };
            await _mediator.Send(commandWithUser, ct);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            var command = new DeleteMediaCommand
            {
                MediaId = id,
                RequesterId = User.GetUserId(),
                IsAdmin = User.IsInRole("Admin")
            };
            await _mediator.Send(command, ct);
            return NoContent();
        }
    }
}