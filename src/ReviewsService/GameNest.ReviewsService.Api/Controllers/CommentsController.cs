using GameNest.ReviewsService.Application.Commands.CommentCommands.AddReply;
using GameNest.ReviewsService.Application.Commands.CommentCommands.CreateComment;
using GameNest.ReviewsService.Application.Commands.CommentCommands.DeleteComment;
using GameNest.ReviewsService.Application.Commands.CommentCommands.DeleteReply;
using GameNest.ReviewsService.Application.Commands.CommentCommands.UpdateCommentText;
using GameNest.ReviewsService.Application.Commands.CommentCommands.UpdateReplyText;
using GameNest.ReviewsService.Application.Queries.CommentQueries.GetCommentById;
using GameNest.ReviewsService.Application.Queries.CommentQueries.GetComments;
using GameNest.ReviewsService.Domain.Entities.Parameters;
using GameNest.ServiceDefaults.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNest.ReviewsService.Api.Controllers
{
    [Authorize]
    public class CommentsController : BaseApiController
    {
        public CommentsController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetCommentByIdQuery { CommentId = id }, ct);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] CommentParameters parameters, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetCommentsQuery(parameters), ct);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommentCommand command, CancellationToken ct)
        {
            var commandWithUser = command with { CustomerId = User.GetUserId() };

            var result = await _mediator.Send(commandWithUser, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}/text")]
        public async Task<IActionResult> UpdateText(string id, [FromBody] UpdateCommentTextCommand command, CancellationToken ct)
        {
            var commandWithUser = command with { CommentId = id, RequesterId = User.GetUserId() };

            await _mediator.Send(commandWithUser, ct);
            return NoContent();
        }

        [HttpPut("{commentId}/replies/{replyId}")]
        public async Task<IActionResult> UpdateReplyText(string commentId, string replyId, [FromBody] UpdateReplyTextCommand command, CancellationToken ct)
        {
            var commandWithUser = command with
            {
                CommentId = commentId,
                ReplyId = replyId,
                RequesterId = User.GetUserId(),
                IsAdmin = User.IsInRole("Admin")
            };

            await _mediator.Send(commandWithUser, ct);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            var command = new DeleteCommentCommand
            {
                CommentId = id,
                RequesterId = User.GetUserId(),
                IsAdmin = User.IsInRole("Admin")
            };

            await _mediator.Send(command, ct);
            return NoContent();
        }

        [HttpPost("{id}/replies")]
        public async Task<IActionResult> AddReply(string id, [FromBody] AddReplyCommand command, CancellationToken ct)
        {
            var commandWithUser = command with
            {
                CommentId = id,
                RequesterId = User.GetUserId()
            };

            await _mediator.Send(commandWithUser, ct);
            return NoContent();
        }

        [HttpDelete("{commentId}/replies/{replyId}")]
        public async Task<IActionResult> DeleteReply(string commentId, string replyId, CancellationToken ct)
        {
            var command = new DeleteReplyCommand
            {
                CommentId = commentId,
                ReplyId = replyId,
                RequesterId = User.GetUserId(),
                IsAdmin = User.IsInRole("Admin")
            };

            await _mediator.Send(command, ct);
            return NoContent();
        }
    }
}
