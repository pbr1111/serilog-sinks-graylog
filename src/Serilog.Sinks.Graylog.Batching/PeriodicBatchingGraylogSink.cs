using Newtonsoft.Json.Linq;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.Graylog.Core;
using Serilog.Sinks.PeriodicBatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Batching
{
    public class PeriodicBatchingGraylogSink : PeriodicBatchingSink
    {
        private readonly ISinkComponentsBuilder _sinkComponentsBuilder;

        public PeriodicBatchingGraylogSink(BatchingGraylogSinkOptions options) : this(options, options.BatchSizeLimit, options.Period, options.QueueLimit)
        {
            _sinkComponentsBuilder = new SinkComponentsBuilder(options);
        }

        public PeriodicBatchingGraylogSink(BatchingGraylogSinkOptions options, int batchSizeLimit, TimeSpan period) : base(batchSizeLimit, period)
        {
            _sinkComponentsBuilder = new SinkComponentsBuilder(options);
        }

        public PeriodicBatchingGraylogSink(BatchingGraylogSinkOptions options, int batchSizeLimit, TimeSpan period, int queueLimit) : base(batchSizeLimit, period, queueLimit)
        {
            _sinkComponentsBuilder = new SinkComponentsBuilder(options);
        }

        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            try
            {
                var converter = _sinkComponentsBuilder.GetGelfConverter();
                var transport = await _sinkComponentsBuilder.GetTransportAsync();
                IEnumerable<Task> sendTasks = events.Select(logEvent =>
                {
                    JObject json = converter.GetGelfJson(logEvent);
                    return transport.SendAsync(json.ToString(Newtonsoft.Json.Formatting.None));
                });
                await Task.WhenAll(sendTasks);
            }
            catch (Exception exc)
            {
                SelfLog.WriteLine("Oops something going wrong {0}", exc);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _sinkComponentsBuilder?.Dispose();
        }
    }
}