using System.Threading.Tasks;
using Moq;
using AutoFixture;
using Serilog.Sinks.Graylog.Core.Transport;
using Serilog.Sinks.Graylog.Core.Transport.Http;
using Xunit;

namespace Serilog.Sinks.Graylog.Core.Tests.Transport.Http
{
    public class HttpTransportFixture
    {
        private readonly Fixture _fixture;

        public HttpTransportFixture()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task WhenCallSend_ThenCallSendWithoutAnyChanges()
        {
            var transportClient = new Mock<ITransportClient<string>>();

            var target = new HttpTransport(transportClient.Object);

            var payload = _fixture.Create<string>();

            await target.SendAsync(payload);

            transportClient.Verify(c => c.SendAsync(payload), Times.Once);
        }
    }
}