using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Entities;
using System.Text.Json.Serialization;

namespace GameNest.ReviewsService.Application.Commands.CommentCommands.AddReply
{
    public record class AddReplyCommand : ICommand
    {
        [JsonIgnore]
        public string? CommentId { get; init; } 
        public Reply Reply { get; init; } = default!;
    }
}