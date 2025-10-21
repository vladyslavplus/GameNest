using System.Diagnostics.Metrics;

namespace GameNest.ServiceDefaults.Metrics
{
    public static class ServiceMetrics
    {
        private static readonly Meter Meter = new("GameNest.Service", "1.0.0");

        public static readonly Counter<long> RequestsTotal =
            Meter.CreateCounter<long>("service_requests_total", "requests", "Total number of processed HTTP/gRPC requests");

        public static readonly Counter<long> FailedRequestsTotal =
            Meter.CreateCounter<long>("service_requests_failed_total", "requests", "Total number of failed requests");

        public static readonly Histogram<double> RequestLatency =
            Meter.CreateHistogram<double>("service_request_latency_seconds", "s", "Latency of requests in seconds");

        public static readonly Counter<long> DatabaseQueriesTotal =
            Meter.CreateCounter<long>("db_queries_total", "queries", "Total number of database queries");

        public static readonly Histogram<double> DatabaseQueryLatency =
            Meter.CreateHistogram<double>("db_query_latency_seconds", "s", "Database query duration in seconds");
    }
}
