using System.Net.Sockets;

namespace Serilog.Sinks.Graylog.Core.Extensions
{
    public static class TcpClientExtensions
    {
        public static bool IsConnected(this TcpClient tcpClient)
        {
            return tcpClient != null && tcpClient.Connected && tcpClient.Client.Connected;
        }
    }
}
