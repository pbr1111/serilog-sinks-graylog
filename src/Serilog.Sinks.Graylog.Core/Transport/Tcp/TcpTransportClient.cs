using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Serilog.Sinks.Graylog.Core.Extensions;

namespace Serilog.Sinks.Graylog.Core.Transport.Tcp
{
    public class TcpTransportClient : ITransportClient<byte[]>
    {
        private readonly IPAddress _address;
        private readonly int _port;
        private TcpClient _client;
        private NetworkStream _stream;

        /// <inheritdoc />
        public TcpTransportClient(IPAddress address, int port)
        {
            _address = address;
            _port = port;
        }


        /// <inheritdoc />
        public async Task SendAsync(byte[] payload)
        {
            await CheckSocketConnectionAsync();

            await _stream.WriteAsync(payload, 0, payload.Length).ConfigureAwait(false);
            await _stream.FlushAsync().ConfigureAwait(false);
        }

        private async Task ConnectAsync()
        {
            await _client.ConnectAsync(_address, _port).ConfigureAwait(false);
            _stream = _client.GetStream();
        }

        private async Task CheckSocketConnectionAsync()
        {
            if (_client.IsConnected())
                return;

            if (_client != null && !_client.IsConnected())
            {
                if (!_client.Connected || !_client.Client.Connected)
                    CloseClient();
            }

            _client = new TcpClient();
            await ConnectAsync();
        }


        private void CloseClient()
        {
#if NETFRAMEWORK
            _client?.Close();
#else
            _client?.Dispose();
#endif
            _stream?.Dispose();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            CloseClient();
        }
    }
}