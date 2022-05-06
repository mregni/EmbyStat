using System;
using EmbyStat.Common.Extensions;
using FluentAssertions;
using Xunit;

namespace Tests.Unit.Extensions;

public class RoundNumberExtensionTests
{
    [Theory]
    [InlineData(2000, 2000)]
    [InlineData(2001, 2000)]
    [InlineData(2002, 2000)]
    [InlineData(2003, 2000)]
    [InlineData(2004, 2000)]
    [InlineData(2005, 2005)]
    [InlineData(2006, 2005)]
    [InlineData(2007, 2005)]
    [InlineData(2008, 2005)]
    [InlineData(2009, 2005)]
    [InlineData(2010, 2010)]
    public void RoundToFiveYear_Should_Round_Year_Correctly(int input, int output)
    {
        DateTime? offset = new DateTime(input, 1, 1, 1, 0, 0, 0);
        var rounded = offset.RoundToFiveYear();

        rounded.Should().Be(output);
    }

    [Fact]
    public void RoundToFiveYear_Should_Handle_Null_Values_Correctly()
    {
        var rounded = ((DateTime?) null).RoundToFiveYear();

        rounded.Should().BeNull();
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0.01, 0)]
    [InlineData(0.02, 0)]
    [InlineData(0.03, 0)]
    [InlineData(0.04, 0)]
    [InlineData(0.05, 5)]
    [InlineData(0.06, 5)]
    [InlineData(0.07, 5)]
    [InlineData(0.08, 5)]
    [InlineData(0.09, 5)]
    [InlineData(0.10, 10)]
    [InlineData(0.50, 50)]
    [InlineData(0.99, 95)]
    [InlineData(1, 100)]
    public void RoundToFive_Should_Round_Number_To_Five(double input, int output)
    {
        var result = input.RoundToFive();
        result.Should().Be(output);
    }

    [Theory]
    [InlineData(0d, 0)]
    [InlineData(0.1d, 0)]
    [InlineData(0.2d, 0)]
    [InlineData(0.3d, 0.5)]
    [InlineData(0.4d, 0.5)]
    [InlineData(0.5d, 0.5)]
    [InlineData(0.6d, 0.5)]
    [InlineData(0.7d, 0.5)]
    [InlineData(0.8d, 1)]
    [InlineData(0.9d, 1)]
    [InlineData(1d, 1)]
    [InlineData(null, null)]
    public void RoundToHalf_Should_Round_Number_To_Half(double? input, double? output)
    {
        decimal? value = null;
        if (input != null)
        {
            value = Convert.ToDecimal(input);
        }
        var result = value.RoundToHalf();
        result.Should().Be(output);
    }
}