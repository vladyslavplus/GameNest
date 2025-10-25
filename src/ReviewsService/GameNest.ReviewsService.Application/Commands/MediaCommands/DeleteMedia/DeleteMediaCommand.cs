using GameNest.ReviewsService.Application.Interfaces.Commands;
using System.Text.Json.Serialization;

namespace GameNest.ReviewsService.Application.Commands.MediaCommands.DeleteMedia
{
    public class DeleteMediaCommand : ICommand
    {
        public string MediaId { get; init; } = default!;
        [JsonIgnore]
        public Guid RequesterId { get; init; }
        [JsonIgnore]
        public bool IsAdmin { get; init; }
    }
}