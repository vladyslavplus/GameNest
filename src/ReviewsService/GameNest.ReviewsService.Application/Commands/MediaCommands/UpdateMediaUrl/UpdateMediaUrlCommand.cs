using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace GameNest.ReviewsService.Application.Commands.MediaCommands.UpdateMediaUrl
{
    public record UpdateMediaUrlCommand : ICommand
    {
        public MediaUrl NewUrl { get; init; } = default!;
        [JsonIgnore]
        public string? MediaId { get; init; }
        [JsonIgnore]
        public Guid RequesterId { get; init; }
    }
}