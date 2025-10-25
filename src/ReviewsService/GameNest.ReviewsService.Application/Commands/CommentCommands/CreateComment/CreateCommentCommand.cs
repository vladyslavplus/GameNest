using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace GameNest.ReviewsService.Application.Commands.CommentCommands.CreateComment
{
    public record CreateCommentCommand(string ReviewId, ReviewText Text) : ICommand<Comment>
    {
        [JsonIgnore]
        public Guid CustomerId { get; init; }
    }
}