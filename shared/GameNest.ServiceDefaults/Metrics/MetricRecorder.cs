using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace GameNest.ServiceDefaults.Metrics
{
    public static class MetricRecorder
    {
        public static async Task RecordOperationAsync(
            Histogram<double> histogram,
            string operation,
            Func<Task> func)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                await func();
                sw.Stop();

                histogram.Record(sw.Elapsed.TotalSeconds, new TagList
                {
                    new(TagConstants.Keys.Operation, operation),
                    new(TagConstants.Keys.Status, TagConstants.Values.Success)
                });
            }
            catch (Exception ex)
            {
                sw.Stop();

                histogram.Record(sw.Elapsed.TotalSeconds, new TagList
                {
                    new(TagConstants.Keys.Operation, operation),
                    new(TagConstants.Keys.Status, TagConstants.Values.Failure),
                    new(TagConstants.Keys.ErrorType, ex.GetType().Name)
                });

                throw;
            }
        }

        public static async Task<T> RecordOperationAsync<T>(
            Histogram<double> histogram,
            string operation,
            Func<Task<T>> func)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                T result = await func();
                sw.Stop();

                histogram.Record(sw.Elapsed.TotalSeconds, new TagList
                {
                    new(TagConstants.Keys.Operation, operation),
                    new(TagConstants.Keys.Status, TagConstants.Values.Success)
                });

                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();

                histogram.Record(sw.Elapsed.TotalSeconds, new TagList
                {
                    new(TagConstants.Keys.Operation, operation),
                    new(TagConstants.Keys.Status, TagConstants.Values.Failure),
                    new(TagConstants.Keys.ErrorType, ex.GetType().Name)
                });

                throw;
            }
        }
    }
}
