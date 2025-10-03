using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Interfaces.Services;
using MediatR;

namespace GameNest.ReviewsService.Application.Commands.MediaCommands.UpdateMediaUrl
{
    public class UpdateMediaUrlCommandHandler : ICommandHandler<UpdateMediaUrlCommand>
    {
        private readonly IMediaService _mediaService;

        public UpdateMediaUrlCommandHandler(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        public async Task<Unit> Handle(UpdateMediaUrlCommand request, CancellationToken cancellationToken)
        {
            await _mediaService.UpdateMediaUrlAsync(request.MediaId!, request.NewUrl, cancellationToken);
            return Unit.Value;
        }
    }
}