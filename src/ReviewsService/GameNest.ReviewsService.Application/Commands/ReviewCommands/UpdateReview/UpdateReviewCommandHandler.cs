using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Interfaces.Services;
using MediatR;

namespace GameNest.ReviewsService.Application.Commands.ReviewCommands.UpdateReview
{
    public class UpdateReviewCommandHandler : ICommandHandler<UpdateReviewCommand>
    {
        private readonly IReviewService _reviewService;

        public UpdateReviewCommandHandler(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<Unit> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
        {
            await _reviewService.UpdateReviewAsync(request.ReviewId!, request.NewText, request.NewRating, cancellationToken);
            return Unit.Value;
        }
    }
}