using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Interfaces.Services;
using MediatR;

namespace GameNest.ReviewsService.Application.Commands.ReviewCommands.AddReplyToReview
{
    public class AddReplyToReviewCommandHandler : ICommandHandler<AddReplyToReviewCommand>
    {
        private readonly IReviewService _reviewService;

        public AddReplyToReviewCommandHandler(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<Unit> Handle(AddReplyToReviewCommand request, CancellationToken cancellationToken)
        {
            await _reviewService.AddReplyToReviewAsync(request.ReviewId!, request.Reply, cancellationToken);
            return Unit.Value;
        }
    }
}