using GameNest.CatalogService.BLL.Cache.Services.Interfaces;
using GameNest.CatalogService.Domain.Entities;
using GameNest.Shared.Events.Genres;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GameNest.CatalogService.BLL.Consumers.Genres
{
    public class GenreDeletedEventConsumer : IConsumer<GenreDeletedEvent>
    {
        private readonly IEntityCacheInvalidationService<Game> _cacheInvalidationService;
        private readonly ILogger<GenreDeletedEventConsumer> _logger;

        public GenreDeletedEventConsumer(
            IEntityCacheInvalidationService<Game> cacheInvalidationService,
            ILogger<GenreDeletedEventConsumer> logger)
        {
            _cacheInvalidationService = cacheInvalidationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GenreDeletedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Received GenreDeletedEvent: GenreId={GenreId}, GenreName={GenreName}",
                message.GenreId, message.GenreName);

            try
            {
                await _cacheInvalidationService.InvalidateAllAsync();

                _logger.LogInformation(
                    "Successfully invalidated game cache after genre deletion: GenreId={GenreId}",
                    message.GenreId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to invalidate cache for GenreDeletedEvent: GenreId={GenreId}",
                    message.GenreId);
                throw;
            }
        }
    }
}