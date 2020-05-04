using Newtonsoft.Json.Linq;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.Graylog.Core;
using System;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog
{
    public class GraylogSink : ILogEventSink, IDisposable
    {
        private readonly ISinkComponentsBuilder _sinkComponentsBuilder;

        public GraylogSink(GraylogSinkOptions options)
        {
            _sinkComponentsBuilder = new SinkComponentsBuilder(options);
        }

        public void Emit(LogEvent logEvent)
        {
            try
            {
                EmitAsync(logEvent).ConfigureAwait(false)
                                   .GetAwaiter()
                                   .GetResult();
            }
            catch (Exception exc)
            {
                SelfLog.WriteLine("Oops something going wrong {0}", exc);
            }
        }

        private async Task EmitAsync(LogEvent logEvent)
        {
            var transport = await _sinkComponentsBuilder.GetTransportAsync();

            JObject json = _sinkComponentsBuilder.GetGelfConverter().GetGelfJson(logEvent);
            string payload = json.ToString(Newtonsoft.Json.Formatting.None);

            await transport.SendAsync(payload);
        }

        public void Dispose()
        {
            _sinkComponentsBuilder?.Dispose();
        }
    }
}
