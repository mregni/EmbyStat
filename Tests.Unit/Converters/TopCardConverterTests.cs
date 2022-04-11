using System.Collections.Generic;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Services.Converters;
using EmbyStat.Services.Models.Cards;
using FluentAssertions;
using Tests.Unit.Builders;
using Xunit;

namespace Tests.Unit.Converters;

public class TopCardConverterTests
{
    [Fact]
    public void ConvertToTopCard_Should_Return_TopCard_From_Movie_List()
    {
        var movies = new[]
        {
            new MovieBuilder("1").Build(),
            new MovieBuilder("2").AddName("The Hobbit").Build()
        };

        var topCard = movies.ConvertToTopCard("Longest movie", "MIN", "Name", ValueTypeEnum.Ticks);

        topCard.Should().NotBeNull();
        topCard.Title.Should().Be("Longest movie");
        topCard.Unit.Should().Be("MIN");
        topCard.UnitNeedsTranslation.Should().BeTrue();
        topCard.ValueType.Should().Be(ValueTypeEnum.Ticks);
        topCard.Values.Length.Should().Be(2);
        topCard.Values[0].Value.Should().Be("The lord of the rings");
        topCard.Values[1].Value.Should().Be("The Hobbit");
    }

    [Fact]
    public void ConvertToTopCard_Should_Return_TopCard_From_Movie_List_With_DateTime_Type()
    {
        var movies = new[]
        {
            new MovieBuilder("1").AddPremiereDate(null).Build(),
            new MovieBuilder("2").Build()
        };

        var topCard = movies.ConvertToTopCard("Longest movie", "MIN", "PremiereDate", ValueTypeEnum.Date);

        topCard.Should().NotBeNull();
        topCard.Title.Should().Be("Longest movie");
        topCard.Unit.Should().Be("MIN");
        topCard.UnitNeedsTranslation.Should().BeTrue();
        topCard.ValueType.Should().Be(ValueTypeEnum.Date);
        topCard.Values.Length.Should().Be(2);
        topCard.Values[0].Value.Should().BeNull();
        topCard.Values[1].Value.Should().Be("2002-01-01T00:00:00");
    }

    [Fact]
    public void ConvertToTopCard_Should_Return_TopCard_From_Movie_List_2()
    {
        var movies = new[]
        {
            new MovieBuilder("1").Build(),
            new MovieBuilder("2").AddName("The Hobbit").Build()
        };

        var topCard = movies.ConvertToTopCard("Longest movie", "MIN", "Name", false);

        topCard.Should().NotBeNull();
        topCard.Title.Should().Be("Longest movie");
        topCard.Unit.Should().Be("MIN");
        topCard.UnitNeedsTranslation.Should().BeFalse();
        topCard.ValueType.Should().Be(ValueTypeEnum.None);
        topCard.Values.Length.Should().Be(2);
        topCard.Values[0].Value.Should().Be("The lord of the rings");
        topCard.Values[1].Value.Should().Be("The Hobbit");
    }

    [Fact]
    public void ConvertToTopCard_Should_Return_TopCard_From_Show_List()
    {
        var shows = new[]
        {
            new ShowBuilder("1").Build(),
            new ShowBuilder("2").AddName("The Hobbit").Build()
        };

        var topCard = shows.ConvertToTopCard("Longest show", "MIN", "Name", ValueTypeEnum.Ticks);

        topCard.Should().NotBeNull();
        topCard.Title.Should().Be("Longest show");
        topCard.Unit.Should().Be("MIN");
        topCard.UnitNeedsTranslation.Should().BeTrue();
        topCard.ValueType.Should().Be(ValueTypeEnum.Ticks);
        topCard.Values.Length.Should().Be(2);
        topCard.Values[0].Value.Should().Be("Chuck");
        topCard.Values[1].Value.Should().Be("The Hobbit");
    }

    [Fact]
    public void ConvertToTopCard_Should_Return_TopCard_From_Show_List_2()
    {
        var shows = new[]
        {
            new ShowBuilder("1").Build(),
            new ShowBuilder("2").AddName("The Hobbit").Build()
        };

        var topCard = shows.ConvertToTopCard("Longest show", "MIN", "Name", false);

        topCard.Should().NotBeNull();
        topCard.Title.Should().Be("Longest show");
        topCard.Unit.Should().Be("MIN");
        topCard.UnitNeedsTranslation.Should().BeFalse();
        topCard.ValueType.Should().Be(ValueTypeEnum.None);
        topCard.Values.Length.Should().Be(2);
        topCard.Values[0].Value.Should().Be("Chuck");
        topCard.Values[1].Value.Should().Be("The Hobbit");
    }

    [Fact]
    public void ConvertToTopCard_Should_Return_TopCard_From_Show_Dictionary()
    {
        var shows = new Dictionary<Show, int>
        {
            { new ShowBuilder("1").Build(), 3 },
            { new ShowBuilder("2").Build(), 2 }
        };

        var topCard = shows.ConvertToTopCard("Longest show", "Title", false);

        topCard.Should().NotBeNull();
        topCard.Title.Should().Be("Longest show");
        topCard.Unit.Should().Be("Title");
        topCard.UnitNeedsTranslation.Should().BeFalse();
        topCard.ValueType.Should().Be(ValueTypeEnum.None);
        topCard.Values.Length.Should().Be(2);
        topCard.Values[0].Value.Should().Be("3");
        topCard.Values[1].Value.Should().Be("2");
    }
}