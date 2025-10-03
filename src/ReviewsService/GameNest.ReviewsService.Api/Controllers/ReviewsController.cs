using GameNest.ReviewsService.Application.Commands.ReviewCommands.AddReplyToReview;
using GameNest.ReviewsService.Application.Commands.ReviewCommands.AddReview;
using GameNest.ReviewsService.Application.Commands.ReviewCommands.DeleteReplyFromReview;
using GameNest.ReviewsService.Application.Commands.ReviewCommands.DeleteReview;
using GameNest.ReviewsService.Application.Commands.ReviewCommands.UpdateReview;
using GameNest.ReviewsService.Application.Queries.ReviewQueries.GetReviewById;
using GameNest.ReviewsService.Application.Queries.ReviewQueries.GetReviews;
using GameNest.ReviewsService.Domain.Entities.Parameters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.ReviewsService.Api.Controllers
{
    public class ReviewsController : BaseApiController
    {
        public ReviewsController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetReviewByIdQuery { ReviewId = id }, ct);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ReviewParameters parameters, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetReviewsQuery { Parameters = parameters }, ct);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddReviewCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateReviewCommand command, CancellationToken ct)
        {
            await _mediator.Send(command with { ReviewId = id }, ct);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            await _mediator.Send(new DeleteReviewCommand { ReviewId = id }, ct);
            return NoContent();
        }

        [HttpPost("{id}/replies")]
        public async Task<IActionResult> AddReply(string id, [FromBody] AddReplyToReviewCommand command, CancellationToken ct)
        {
            await _mediator.Send(command with { ReviewId = id }, ct);
            return NoContent();
        }

        [HttpDelete("{reviewId}/replies/{replyId}")]
        public async Task<IActionResult> DeleteReply(string reviewId, string replyId, CancellationToken ct)
        {
            await _mediator.Send(new DeleteReplyFromReviewCommand
            {
                ReviewId = reviewId,
                ReplyId = replyId
            }, ct);
            return NoContent();
        }
    }
}