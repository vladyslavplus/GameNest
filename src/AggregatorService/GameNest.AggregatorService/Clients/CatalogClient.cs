using GameNest.AggregatorService.DTOs.Catalog;

namespace GameNest.AggregatorService.Clients
{
    public class CatalogClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CatalogClient> _logger;

        public CatalogClient(HttpClient httpClient, ILogger<CatalogClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<GameDto>?> GetGamesAsync(CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/catalog/games", ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch games from CatalogService. Status code: {StatusCode}", response.StatusCode);
                    return null;
                }

                var games = await response.Content.ReadFromJsonAsync<IEnumerable<GameDto>>(cancellationToken: ct);
                return games;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching games from CatalogService.");
                return null;
            }
        }

        public async Task<GameDto?> GetGameByIdAsync(Guid gameId, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/catalog/games/{gameId}", ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch game {GameId}. Status code: {StatusCode}", gameId, response.StatusCode);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<GameDto>(cancellationToken: ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching game {GameId}", gameId);
                return null;
            }
        }
    }
}
