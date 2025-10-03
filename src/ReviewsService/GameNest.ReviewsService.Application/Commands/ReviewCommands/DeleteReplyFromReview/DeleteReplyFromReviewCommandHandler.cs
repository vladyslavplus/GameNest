using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Interfaces.Services;
using MediatR;

namespace GameNest.ReviewsService.Application.Commands.ReviewCommands.DeleteReplyFromReview
{
    public class DeleteReplyFromReviewCommandHandler : ICommandHandler<DeleteReplyFromReviewCommand>
    {
        private readonly IReviewService _reviewService;

        public DeleteReplyFromReviewCommandHandler(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<Unit> Handle(DeleteReplyFromReviewCommand request, CancellationToken cancellationToken)
        {
            await _reviewService.DeleteReplyFromReviewAsync(request.ReviewId, request.ReplyId, cancellationToken);
            return Unit.Value;
        }
    }
}