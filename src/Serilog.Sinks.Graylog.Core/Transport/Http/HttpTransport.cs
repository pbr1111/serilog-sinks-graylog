using System;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Core.Transport.Http
{
    public class HttpTransport : ITransport
    {
        private readonly ITransportClient<string> _transportClient;

        public HttpTransport(ITransportClient<string> transportClient)
        {
            _transportClient = transportClient;
        }

        public async Task SendAsync(string message)
        {
            await _transportClient.SendAsync(message);
        }

        public void Dispose()
        {
            _transportClient?.Dispose();
        }
    }
}