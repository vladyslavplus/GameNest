using GameNest.ServiceDefaults.Metrics;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace GameNest.CatalogService.BLL.Metrics
{
    public static class GameMetrics
    {
        private static readonly Meter Meter = new("GameNest.CatalogService.Games", "1.0.0");

        public static readonly Counter<long> GamesCreated =
            Meter.CreateCounter<long>("games.created_total", "{games}", "Total number of games created");

        public static readonly Counter<long> GamesUpdated =
            Meter.CreateCounter<long>("games.updated_total", "{games}", "Total number of games updated");

        public static readonly Counter<long> GamesDeleted =
            Meter.CreateCounter<long>("games.deleted_total", "{games}", "Total number of games deleted");

        public static readonly Counter<long> GamesFetched =
            Meter.CreateCounter<long>("games.fetched_total", "{requests}", "Total number of game retrievals (cache or DB)");

        public static readonly Histogram<double> OperationLatency =
            Meter.CreateHistogram<double>("games.operation_duration_seconds", "s", "Duration of game operations in seconds");

        public static void RecordFetch(string operation, bool found)
        {
            GamesFetched.Add(1, new TagList
            {
                new(TagConstants.Keys.Operation, operation),
                new(TagConstants.Keys.Found, found.ToString().ToLower())
            });
        }
    }
}
