using GameNest.ReviewsService.Application.Interfaces.Commands;

namespace GameNest.ReviewsService.Application.Commands.MediaCommands.DeleteMedia
{
    public class DeleteMediaCommand : ICommand
    {
        public string MediaId { get; init; } = default!;
    }
}