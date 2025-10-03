using GameNest.ReviewsService.Application.Interfaces.Commands;

namespace GameNest.ReviewsService.Application.Commands.ReviewCommands.DeleteReview
{
    public class DeleteReviewCommand : ICommand
    {
        public string ReviewId { get; init; } = default!;
    }
}