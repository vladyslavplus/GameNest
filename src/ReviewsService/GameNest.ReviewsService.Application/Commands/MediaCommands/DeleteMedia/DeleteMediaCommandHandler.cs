using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Interfaces.Services;
using MediatR;

namespace GameNest.ReviewsService.Application.Commands.MediaCommands.DeleteMedia
{
    public class DeleteMediaCommandHandler : ICommandHandler<DeleteMediaCommand>
    {
        private readonly IMediaService _mediaService;

        public DeleteMediaCommandHandler(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        public async Task<Unit> Handle(DeleteMediaCommand request, CancellationToken cancellationToken)
        {
            await _mediaService.DeleteMediaAsync(
                request.RequesterId,
                request.MediaId,
                request.IsAdmin,
                cancellationToken
            );
            return Unit.Value;
        }
    }
}