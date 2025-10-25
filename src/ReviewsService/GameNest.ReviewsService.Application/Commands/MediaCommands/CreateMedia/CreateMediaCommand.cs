using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace GameNest.ReviewsService.Application.Commands.MediaCommands.CreateMedia
{
    public record CreateMediaCommand(string GameId, MediaUrl Url) : ICommand<Media>
    {
        [JsonIgnore]
        public Guid CustomerId { get; init; }
    }
}