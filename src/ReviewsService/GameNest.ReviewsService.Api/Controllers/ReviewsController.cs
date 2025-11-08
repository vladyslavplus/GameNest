using GameNest.ReviewsService.Application.Commands.ReviewCommands.AddReview;
using GameNest.ReviewsService.Application.Commands.ReviewCommands.DeleteReview;
using GameNest.ReviewsService.Application.Commands.ReviewCommands.UpdateReview;
using GameNest.ReviewsService.Application.Queries.ReviewQueries.GetReviewById;
using GameNest.ReviewsService.Application.Queries.ReviewQueries.GetReviews;
using GameNest.ReviewsService.Domain.Entities.Parameters;
using GameNest.ServiceDefaults.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.ReviewsService.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ReviewsController : BaseApiController
    {
        public ReviewsController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetReviewByIdQuery { ReviewId = id }, ct);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] ReviewParameters parameters, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetReviewsQuery { Parameters = parameters }, ct);
            return Ok(result);
        }

        [HttpPost]
        [RequirePermission("reviews:write")]
        public async Task<IActionResult> Create([FromBody] AddReviewCommand command, CancellationToken ct)
        {
            var commandWithUser = command with
            {
                CustomerId = User.GetUserId()
            };

            var result = await _mediator.Send(commandWithUser, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [RequirePermission("reviews:update")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateReviewCommand command, CancellationToken ct)
        {
            var commandWithUser = command with
            {
                ReviewId = id,
                RequesterId = User.GetUserId()
            };

            await _mediator.Send(commandWithUser, ct);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [RequirePermission("reviews:delete")]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            var command = new DeleteReviewCommand
            {
                ReviewId = id,
                RequesterId = User.GetUserId(),
                IsAdmin = User.IsInRole("Admin")
            };

            await _mediator.Send(command, ct);
            return NoContent();
        }
    }
}