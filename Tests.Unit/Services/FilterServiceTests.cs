using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Core.Filters;
using EmbyStat.Core.Filters.Interfaces;
using EmbyStat.Core.Movies.Interfaces;
using EmbyStat.Core.Shows.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Unit.Services;

public class FilterServiceTests
{
    private readonly Mock<IMovieRepository> _movieRepositoryMock;
    private readonly Mock<IFilterRepository> _filterRepositoryMock;
    private readonly Mock<IShowRepository> _showRepositoryMock;

    private FilterValues _filterValues;

    public FilterServiceTests()
    {
        _movieRepositoryMock = new Mock<IMovieRepository>();
        _filterRepositoryMock = new Mock<IFilterRepository>();
        _showRepositoryMock = new Mock<IShowRepository>();
    }

    private void SetupMovieRepository()
    {
        _movieRepositoryMock
            .Setup(x => x.CalculateSubtitleFilterValues())
            .Returns(new[]
                {new LabelValuePair {Label = "6", Value = "6"}, new LabelValuePair {Label = "7", Value = "7"}});
        _movieRepositoryMock
            .Setup(x => x.CalculateGenreFilterValues())
            .Returns(new[]
                {new LabelValuePair {Label = "8", Value = "8"}, new LabelValuePair {Label = "9", Value = "9"}});
        _movieRepositoryMock
            .Setup(x => x.CalculateContainerFilterValues())
            .Returns(new[]
                {new LabelValuePair {Label = "10", Value = "10"}, new LabelValuePair {Label = "11", Value = "11"}}); 
    }

    private FilterService CreateFilterService(LibraryType type, string field)
    {
        _filterValues = new FilterValues
        {
            Id = 1,
            Field = field,
            Values = new[]
                {new LabelValuePair {Label = "2", Value = "2"}, new LabelValuePair {Label = "3", Value = "3"}}
        };

        _filterRepositoryMock
            .Setup(x => x.Get(type, field))
            .ReturnsAsync(_filterValues);
        _filterRepositoryMock
            .Setup(x => x.Insert(It.IsAny<FilterValues>()));

        return new FilterService(_filterRepositoryMock.Object, _movieRepositoryMock.Object, _showRepositoryMock.Object);
    }

    [Fact]
    public async Task Get_Should_Return_Existing_Movie_FilterValues()
    {
        var filterService = CreateFilterService(LibraryType.Movies, "subtitle");
        var values = await filterService.GetFilterValues(LibraryType.Movies, "subtitle");
        values.Should().NotBeNull();

        values.Id.Should().Be(_filterValues.Id);
        values.Field.Should().Be(_filterValues.Field);
        values.Values.Length.Should().Be(2);
        values.Values[0].Label.Should().Be("2");
        values.Values[0].Value.Should().Be("2");
        values.Values[1].Label.Should().Be("3");
        values.Values[1].Value.Should().Be("3");

        _filterRepositoryMock.Verify(x => x.Get(LibraryType.Movies, "subtitle"), Times.Once);
    }

    [Fact]
    public async Task Get_Should_Calculate_Missing_Subtitle_FilterValues()
    {
        SetupMovieRepository();
        var filterService = CreateFilterService(LibraryType.TvShow, "subtitle");

        var values = await filterService.GetFilterValues(LibraryType.Movies, "subtitle");

        values.Field.Should().Be(_filterValues.Field);
        values.Values.Length.Should().Be(2);
        values.Values[0].Label.Should().Be("6");
        values.Values[0].Value.Should().Be("6");
        values.Values[1].Label.Should().Be("7");
        values.Values[1].Value.Should().Be("7");

        _filterRepositoryMock.Verify(x => x.Insert(It.IsAny<FilterValues>()));
        _filterRepositoryMock.Verify(x => x.Get(LibraryType.Movies, "subtitle"));
        _filterRepositoryMock.VerifyNoOtherCalls();
        _movieRepositoryMock.Verify(x => x.CalculateSubtitleFilterValues(), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Get_Should_Calculate_Missing_Genre_FilterValues()
    {
        SetupMovieRepository();
        var filterService = CreateFilterService(LibraryType.TvShow, "genre");

        var values = await filterService.GetFilterValues(LibraryType.Movies, "genre");

        values.Field.Should().Be(_filterValues.Field);
        values.Values.Length.Should().Be(2);
        values.Values[0].Value.Should().Be("8");
        values.Values[0].Label.Should().Be("8");
        values.Values[1].Value.Should().Be("9");
        values.Values[1].Label.Should().Be("9");

        _filterRepositoryMock.Verify(x => x.Insert(It.IsAny<FilterValues>()));
        _filterRepositoryMock.Verify(x => x.Get(LibraryType.Movies, "genre"));
        _filterRepositoryMock.VerifyNoOtherCalls();
        _movieRepositoryMock.Verify(x => x.CalculateGenreFilterValues(), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Get_Should_Calculate_Missing_Container_FilterValues()
    {
        SetupMovieRepository();
        var filterService = CreateFilterService(LibraryType.TvShow, "container");

        var values = await filterService.GetFilterValues(LibraryType.Movies, "container");

        values.Field.Should().Be(_filterValues.Field);
        values.Values.Length.Should().Be(2);
        values.Values[0].Label.Should().Be("10");
        values.Values[0].Value.Should().Be("10");
        values.Values[1].Label.Should().Be("11");
        values.Values[1].Value.Should().Be("11");

        _filterRepositoryMock.Verify(x => x.Insert(It.IsAny<FilterValues>()));
        _movieRepositoryMock.Verify(x => x.CalculateContainerFilterValues(), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Get_Should_Return_Null_If_Unknown_Field_Is_Given()
    {
        SetupMovieRepository();
        var filterService = CreateFilterService(LibraryType.TvShow, "strangeField");

        var values = await filterService.GetFilterValues(LibraryType.Movies, "strangeField");
        values.Should().BeNull();

        _filterRepositoryMock.Verify(x => x.Insert(It.IsAny<FilterValues>()), Times.Never);
        _movieRepositoryMock.VerifyNoOtherCalls();
    }
}