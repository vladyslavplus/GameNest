using GameNest.ReviewsService.Application.Commands.CommentCommands.AddReply;
using GameNest.ReviewsService.Application.Commands.CommentCommands.CreateComment;
using GameNest.ReviewsService.Application.Commands.CommentCommands.DeleteComment;
using GameNest.ReviewsService.Application.Commands.CommentCommands.DeleteReply;
using GameNest.ReviewsService.Application.Commands.CommentCommands.UpdateCommentText;
using GameNest.ReviewsService.Application.Queries.CommentQueries.GetCommentById;
using GameNest.ReviewsService.Application.Queries.CommentQueries.GetComments;
using GameNest.ReviewsService.Domain.Entities.Parameters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.ReviewsService.Api.Controllers
{
    public class CommentsController : BaseApiController
    {
        public CommentsController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetCommentByIdQuery { CommentId = id }, ct);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CommentParameters parameters, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetCommentsQuery(parameters), ct);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommentCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}/text")]
        public async Task<IActionResult> UpdateText(string id, [FromBody] UpdateCommentTextCommand command, CancellationToken ct)
        {
            await _mediator.Send(command with { CommentId = id }, ct);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            await _mediator.Send(new DeleteCommentCommand { CommentId = id }, ct);
            return NoContent();
        }

        [HttpPost("{id}/replies")]
        public async Task<IActionResult> AddReply(string id, [FromBody] AddReplyCommand command, CancellationToken ct)
        {
            await _mediator.Send(command with { CommentId = id }, ct);
            return NoContent();
        }

        [HttpDelete("{commentId}/replies/{replyId}")]
        public async Task<IActionResult> DeleteReply(string commentId, string replyId, CancellationToken ct)
        {
            await _mediator.Send(new DeleteReplyCommand { CommentId = commentId, ReplyId = replyId }, ct);
            return NoContent();
        }
    }
}
