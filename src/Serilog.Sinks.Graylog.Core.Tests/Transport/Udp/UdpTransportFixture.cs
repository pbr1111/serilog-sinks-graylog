﻿using System.Collections.Generic;
using System.Linq;
using Moq;
using AutoFixture;
using Serilog.Sinks.Graylog.Core.Extensions;
using Serilog.Sinks.Graylog.Core.Transport;
using Serilog.Sinks.Graylog.Core.Transport.Udp;
using Xunit;
using System.Threading.Tasks;

namespace Serilog.Sinks.Graylog.Core.Tests.Transport.Udp
{
    public class UdpTransportFixture
    {
        [Fact]
        public async Task WhenSend_ThenCallMethods()
        {
            var transportClient = new Mock<ITransportClient<byte[]>>();
            var dataToChunkConverter = new Mock<IDataToChunkConverter>();
            var fixture = new Fixture();

            var stringData = fixture.Create<string>();

            byte[] data = await stringData.CompressAsync();

            List<byte[]> chunks = fixture.CreateMany<byte[]>(3).ToList();

            dataToChunkConverter.Setup(c => c.ConvertToChunks(data)).Returns(chunks);

            UdpTransport target = new UdpTransport(transportClient.Object, dataToChunkConverter.Object);

            await target.SendAsync(stringData);

            dataToChunkConverter.Verify(c => c.ConvertToChunks(data), Times.Once);

            foreach (byte[] chunk in chunks)
            {
                transportClient.Verify(c => c.SendAsync(chunk), Times.Once);
            }
            
        }
    }
}