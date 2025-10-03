using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace GameNest.ReviewsService.Application.Commands.MediaCommands.UpdateMediaUrl
{
    public record UpdateMediaUrlCommand : ICommand
    {
        [JsonIgnore]
        public string? MediaId { get; init; }
        public MediaUrl NewUrl { get; init; } = default!;
    }
}