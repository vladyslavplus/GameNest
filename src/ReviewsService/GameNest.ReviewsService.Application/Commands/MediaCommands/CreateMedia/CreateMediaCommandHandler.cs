using GameNest.ReviewsService.Application.Interfaces.Commands;
using GameNest.ReviewsService.Domain.Entities;
using GameNest.ReviewsService.Domain.Interfaces.Services;

namespace GameNest.ReviewsService.Application.Commands.MediaCommands.CreateMedia
{
    public class CreateMediaCommandHandler : ICommandHandler<CreateMediaCommand, Media>
    {
        private readonly IMediaService _mediaService;

        public CreateMediaCommandHandler(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        public async Task<Media> Handle(CreateMediaCommand request, CancellationToken cancellationToken)
        {
            var media = new Media(
                request.GameId,
                request.CustomerId,
                request.Url
            );

            await _mediaService.AddMediaAsync(media, cancellationToken);
            return media;
        }
    }
}