using System;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Query;
using EmbyStat.Repositories;
using FluentAssertions;
using Tests.Unit.Builders;
using Xunit;

namespace Tests.Unit.Extensions;

public class MovieExtensionTests
{
    [Fact]
    public void GenerateFullMovieQuery_Without_Filters_Should_Return_Query()
    {
        var filters = Array.Empty<Filter>();
        
        var result = MovieExtensions.GenerateFullMovieQuery(filters, "Name", "asc");
        result.Should().Be(@"
SELECT m.*, g.*, aus.*, vis.*, sus.*, mes.*
FROM Movies as m
LEFT JOIN GenreMovie AS gm ON (gm.MoviesId = m.Id)
LEFT JOIN Genres AS g ON (gm.GenresId = g.Id)
LEFT JOIN AudioStream AS aus ON (aus.MovieId = m.Id)
LEFT JOIN VideoStream AS vis ON (vis.MovieId = m.Id)
LEFT JOIN SubtitleStream AS sus ON (sus.MovieId = m.Id)
LEFT JOIN MediaSource AS mes ON (mes.MovieId = m.Id)
WHERE 1=1 ORDER BY Name ASC ");
    }
    
    [Fact]
    public void GenerateFullMovieQuery_With_Filters_Should_Return_Query()
    {
        var filters = new []
        {
            new FilterBuilder("Name", "==", "Lord").Build(),
        };
        
        var result = MovieExtensions.GenerateFullMovieQuery(filters, "Name", "asc");
        result.Should().Be(@"
SELECT m.*, g.*, aus.*, vis.*, sus.*, mes.*
FROM Movies as m
LEFT JOIN GenreMovie AS gm ON (gm.MoviesId = m.Id)
LEFT JOIN Genres AS g ON (gm.GenresId = g.Id)
LEFT JOIN AudioStream AS aus ON (aus.MovieId = m.Id)
LEFT JOIN VideoStream AS vis ON (vis.MovieId = m.Id)
LEFT JOIN SubtitleStream AS sus ON (sus.MovieId = m.Id)
LEFT JOIN MediaSource AS mes ON (mes.MovieId = m.Id)
WHERE 1=1 AND (m.SortName = 'Lord' OR m.Name = 'Lord') ORDER BY Name ASC ");
    }
}