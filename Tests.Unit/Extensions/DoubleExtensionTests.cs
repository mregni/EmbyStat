using EmbyStat.Common.Extensions;
using FluentAssertions;
using Xunit;

namespace Tests.Unit.Extensions;

public class DoubleExtensionTests
{
    [Fact]
    public void FormatToDotDecimalString_Should_Return_String()
    {
        var value = 6.2d;
        var result = value.FormatToDotDecimalString();
        result.Should().NotBeNull();
        result.Should().Be("6.2");
    }
}