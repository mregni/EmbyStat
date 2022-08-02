using System;
using EmbyStat.Common;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Query;
using FluentAssertions;
using Tests.Unit.Builders;
using Xunit;

namespace Tests.Unit.Extensions;

public class ShowExtensionTests
{
    [Fact]
    public void GenerateCountQuery_Should_Return_Query_Without_Filters()
    {
        var query = ShowExtensions.GenerateCountQuery(Array.Empty<Filter>());
        
        var wantedQuery = $@"
SELECT COUNT() AS Count
FROM {Constants.Tables.Shows} as s
WHERE 1=1 ";
        query.Should().NotBeNullOrWhiteSpace();
        query.Should().Be(wantedQuery);
    }
    
    [Fact]
    public void GenerateFullShowQuery_Should_Return_Query()
    {
        var query = ShowExtensions.GenerateFullShowQuery();
        
        var wantedQuery = $@"
SELECT s.*, se.*, e.*
FROM {Constants.Tables.Shows} as s
LEFT JOIN {Constants.Tables.Seasons} AS se ON (s.Id = se.ShowId)
LEFT JOIN {Constants.Tables.Episodes} AS e ON (se.Id = e.SeasonId)
WHERE 1=1 ";
        query.Should().NotBeNullOrWhiteSpace();
        query.Should().Be(wantedQuery);
    }
    
    [Fact]
    public void GenerateFullShowWithGenresQuery_Should_Return_Query()
    {
        var query = ShowExtensions.GenerateFullShowWithGenresQuery();
        
        var wantedQuery = $@"
SELECT s.*, g.*, se.*, e.*
FROM {Constants.Tables.Shows} as s
LEFT JOIN {Constants.Tables.GenreShow} AS gs ON (gs.ShowsId = s.Id)
LEFT JOIN {Constants.Tables.Genres} AS g ON (gs.GenresId = g.Id)
LEFT JOIN {Constants.Tables.Seasons} AS se ON (s.Id = se.ShowId)
LEFT JOIN {Constants.Tables.Episodes} AS e ON (se.Id = e.SeasonId)
WHERE 1=1 ";
        query.Should().NotBeNullOrWhiteSpace();
        query.Should().Be(wantedQuery);
    }

    [Theory]
    [InlineData("sortName", "asc", "ASC")]
    [InlineData("Name", "desc", "DESC")]
    public void GenerateShowPageQuery_Should_Return_Query_With_SortField(string field, string order, string orderResult)
    {
        var query = ShowExtensions.GenerateShowPageQuery(Array.Empty<Filter>(), field, order);
        
        var wantedQuery = $@"
SELECT s.*, g.*, se.*, e.*
FROM {Constants.Tables.Shows} as s
LEFT JOIN {Constants.Tables.GenreShow} AS gs ON (gs.ShowsId = s.Id)
LEFT JOIN {Constants.Tables.Genres} AS g ON (gs.GenresId = g.Id)
LEFT JOIN {Constants.Tables.Seasons} AS se ON (s.Id = se.ShowId)
LEFT JOIN {Constants.Tables.Episodes} AS e ON (se.Id = e.SeasonId)
WHERE 1=1 ORDER BY {field.FirstCharToUpper()} {orderResult} ";
        query.Should().NotBeNullOrWhiteSpace();
        query.Should().Be(wantedQuery);
    }
    
    [Fact]
    public void GenerateShowPageQuery_Should_Return_Query_Without_SortField()
    {
        var query = ShowExtensions.GenerateShowPageQuery(Array.Empty<Filter>(), "", "");
        
        var wantedQuery = $@"
SELECT s.*, g.*, se.*, e.*
FROM {Constants.Tables.Shows} as s
LEFT JOIN {Constants.Tables.GenreShow} AS gs ON (gs.ShowsId = s.Id)
LEFT JOIN {Constants.Tables.Genres} AS g ON (gs.GenresId = g.Id)
LEFT JOIN {Constants.Tables.Seasons} AS se ON (s.Id = se.ShowId)
LEFT JOIN {Constants.Tables.Episodes} AS e ON (se.Id = e.SeasonId)
WHERE 1=1 ";
        query.Should().NotBeNullOrWhiteSpace();
        query.Should().Be(wantedQuery);
    }

    [Fact]
    public void GetShowSize_Should_Return_Size()
    {
        var show = new ShowBuilder("1").Build();
        var size = show.GetShowSize();
        size.Should().Be(404);
    }
    
    [Fact]
    public void GetShowSize_Should_Filter_Out_Virtual_Episodes()
    {
        var show = new ShowBuilder("1")
            .AddMissingEpisodes(1, 1)
            .Build();
        var size = show.GetShowSize();
        size.Should().Be(404);
    }
}