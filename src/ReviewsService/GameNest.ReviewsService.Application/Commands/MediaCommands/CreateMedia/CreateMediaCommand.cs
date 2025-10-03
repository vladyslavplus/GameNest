using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.ValueObjects;

namespace GameNest.ReviewsService.Application.Commands.MediaCommands.CreateMedia
{
    public class CreateMediaCommand : ICommand<Media>
    {
        public string GameId { get; init; } = default!;
        public string CustomerId { get; init; } = default!;
        public MediaUrl Url { get; init; } = default!;
    }
}