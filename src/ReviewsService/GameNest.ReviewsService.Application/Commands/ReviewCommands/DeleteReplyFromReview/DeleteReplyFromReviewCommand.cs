using GameNest.ReviewsService.Application.Interfaces.Commands;

namespace GameNest.ReviewsService.Application.Commands.ReviewCommands.DeleteReplyFromReview
{
    public class DeleteReplyFromReviewCommand : ICommand
    {
        public string ReviewId { get; init; } = default!;
        public string ReplyId { get; init; } = default!;
    }
}