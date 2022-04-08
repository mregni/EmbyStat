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
        var context = new EsDbContext();
        var filters = new Filter[0];
        // {
        //     new FilterBuilder("Name", "==", "Lord").Build(),
        // };
        
        var result = context.Movies.GenerateFullMovieQuery(filters, "Name", "asc");
        result.Should().Be(@"
SELECT m.*, g.*, aus.*, vis.*, sus.*, mes.*
FROM Movies as m
LEFT JOIN GenreMovie AS gm ON (gm.MoviesId = m.Id)
LEFT JOIN Genres AS g ON (gm.GenresId = g.Id)
LEFT JOIN AudioStreams AS aus ON (aus.MovieId = m.Id)
LEFT JOIN VideoStreams AS vis ON (vis.MovieId = m.Id)
LEFT JOIN SubtitleStreams AS sus ON (sus.MovieId = m.Id)
LEFT JOIN MediaSources AS mes ON (mes.MovieId = m.Id)
WHERE 1=1 ORDER BY Name ASC ");
    }
    
    [Fact]
    public void GenerateFullMovieQuery_With_Filters_Should_Return_Query()
    {
        var context = new EsDbContext();
        var filters = new []
        {
            new FilterBuilder("Name", "==", "Lord").Build(),
        };
        
        var result = context.Movies.GenerateFullMovieQuery(filters, "Name", "asc");
        result.Should().Be(@"
SELECT m.*, g.*, aus.*, vis.*, sus.*, mes.*
FROM Movies as m
LEFT JOIN GenreMovie AS gm ON (gm.MoviesId = m.Id)
LEFT JOIN Genres AS g ON (gm.GenresId = g.Id)
LEFT JOIN AudioStreams AS aus ON (aus.MovieId = m.Id)
LEFT JOIN VideoStreams AS vis ON (vis.MovieId = m.Id)
LEFT JOIN SubtitleStreams AS sus ON (sus.MovieId = m.Id)
LEFT JOIN MediaSources AS mes ON (mes.MovieId = m.Id)
WHERE 1=1 AND (m.SortName = 'Lord' OR m.Name = 'Lord') ORDER BY Name ASC ");
    }
}