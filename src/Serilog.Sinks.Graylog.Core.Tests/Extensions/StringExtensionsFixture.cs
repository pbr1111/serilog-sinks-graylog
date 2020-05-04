using FluentAssertions;
using Serilog.Sinks.Graylog.Core.Extensions;
using System.Threading.Tasks;
using Xunit;

namespace Serilog.Sinks.Graylog.Core.Tests.Extensions
{
    public class StringExtensionsFixture
    {
        [Fact]
        public async Task WhenCompressMessage_ThenResultShoouldBeExpected()
        {
            var giwen = "Some string";
            var expected = new byte[]
            {
                31,139,8,0,0,0,0,0,0,11,11,206,207,77,85,40,46,41,202,204,75,7,0,142,183,209,127,11,0,0,0
            };

            byte[] actual = await giwen.CompressAsync();
            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("SomeTestString", "Some", 4)]
        [InlineData("SomeTestString", "SomeTest", 8)]
        [InlineData("SomeTestString", "SomeTestString", 200)]
        public void WhenShortMessage_ThenResultShouldBeExpected(string given, string expected, int length)
        {
            var actual = given.Truncate(length);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}