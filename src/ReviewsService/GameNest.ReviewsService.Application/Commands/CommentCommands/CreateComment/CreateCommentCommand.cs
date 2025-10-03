using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.ValueObjects;

namespace GameNest.ReviewsService.Application.Commands.CommentCommands.CreateComment
{
    public class CreateCommentCommand : ICommand<Comment>
    {
        public string ReviewId { get; init; } = default!;
        public string CustomerId { get; init; } = default!;
        public ReviewText Text { get; init; } = default!;
    }
}