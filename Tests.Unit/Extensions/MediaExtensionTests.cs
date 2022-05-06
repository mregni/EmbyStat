using EmbyStat.Common.Extensions;
using FluentAssertions;
using Xunit;

namespace Tests.Unit.Extensions;

public class MediaExtensionTests
{
    [Theory]
    [InlineData("Movies", "ASC", 5)]
    [InlineData("Shows", "ASC", 5)]
    [InlineData("Movies", "DESC", 2)]
    [InlineData("Shows", "DESC", 2)]
    public void GenerateGetPremieredListQuery_Should_Generate_Query_String(string table, string orderDirection, int count)
    {
        var query = MediaExtensions.GenerateGetPremieredListQuery(count, orderDirection, table);
        query.Should().NotBeNull();
        query.Should().Be($@"SELECT s.*
FROM {table} AS s
WHERE s.PremiereDate IS NOT NULL 
ORDER BY PremiereDate {orderDirection}
LIMIT {count}");
    }
    
    [Theory]
    [InlineData("Movies", "ASC", 5)]
    [InlineData("Shows", "ASC", 5)]
    [InlineData("Movies", "DESC", 2)]
    [InlineData("Shows", "DESC", 2)]
    public void GenerateGetCommunityRatingListQuery_Should_Generate_Query_String(string table, string orderDirection, int count)
    {
        var query = MediaExtensions.GenerateGetCommunityRatingListQuery(count, orderDirection, table);
        query.Should().NotBeNull();
        query.Should().Be($@"SELECT s.*
FROM {table} AS s
WHERE s.CommunityRating IS NOT NULL
ORDER BY CommunityRating {orderDirection}
LIMIT {count}");
    }
}