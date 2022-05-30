using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Controllers;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Controllers.Movie;
using EmbyStat.Core.Movies;
using EmbyStat.Core.Movies.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tests.Unit.Builders;
using Xunit;

namespace Tests.Unit.Controllers;

public class MovieControllerTests
{
    private readonly MovieController _subject;
    private readonly Mock<IMovieService> _movieServiceMock;
    private readonly List<Library> _collections;
    private readonly List<TopCard> _movieCards;
    private readonly Movie _movie;

    public MovieControllerTests()
    {
        _collections = new List<Library>
        {
            new LibraryBuilder(1, LibraryType.Movies).Build(),
            new LibraryBuilder(2, LibraryType.Movies).Build()
        };

        _movie = new MovieBuilder("1").Build();

        _movieCards = new List<TopCard>
        {
            new() {Title = "The lord of the rings"}
        };

        var movieStatistics = new MovieStatistics
        {
            TopCards = _movieCards
        };

        _movieServiceMock = new Mock<IMovieService>();
        _movieServiceMock.Setup(x => x.GetMovieLibraries()).ReturnsAsync(_collections);
        _movieServiceMock.Setup(x => x.GetStatistics()).ReturnsAsync(movieStatistics);
        _movieServiceMock.Setup(x => x.GetMovie(It.IsAny<string>()))
            .ReturnsAsync(_movie);

        var mapperMock = new Mock<IMapper>();
        mapperMock
            .Setup(x => x.Map<MovieStatisticsViewModel>(It.IsAny<MovieStatistics>()))
            .Returns(new MovieStatisticsViewModel
                {TopCards = new List<TopCardViewModel> {new() {Title = "The lord of the rings"}}});
        mapperMock
            .Setup(x => x.Map<IList<LibraryViewModel>>(It.IsAny<List<Library>>()))
            .Returns(
                new List<LibraryViewModel>
                {
                    new()
                    {
                        Name = "collection1",
                        Primary = "image1",
                        Type = (int) LibraryType.Movies
                    },
                    new()
                    {
                        Name = "collection2",
                        Primary = "image2",
                        Type = (int) LibraryType.Movies
                    }
                });
        mapperMock
            .Setup(x => x.Map<MovieViewModel>(It.IsAny<Movie>()))
            .Returns(new MovieViewModel
            {
                Id = "1"
            });
        _subject = new MovieController(_movieServiceMock.Object, mapperMock.Object);
    }

    [Fact]
    public async Task AreMovieCollectionsReturned()
    {
        var result = await _subject.GetLibraries();
        var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
        var list = resultObject.Should().BeOfType<List<LibraryViewModel>>().Subject;

        list.Count.Should().Be(2);
        list[0].Name.Should().Be(_collections[0].Name);
        list[1].Name.Should().Be(_collections[1].Name);
        _movieServiceMock.Verify(x => x.GetMovieLibraries(), Times.Once);
        _movieServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task AreMovieStatsReturned()
    {
        var result = await _subject.GetGeneralStats();
        var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
        var stat = resultObject.Should().BeOfType<MovieStatisticsViewModel>().Subject;

        stat.Should().NotBeNull();
        stat.TopCards.Count.Should().Be(1);
        stat.TopCards[0].Title.Should().Be(_movieCards[0].Title);
        _movieServiceMock.Verify(x => x.GetStatistics(), Times.Once);
        _movieServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMovie_Should_Return_NotFound()
    {
        var mapper = CreateMapper();
        var serviceMock = new Mock<IMovieService>();
        serviceMock.Setup(x => x.GetMovie(It.IsAny<string>())).ReturnsAsync((Movie) null);

        var subject = new MovieController(serviceMock.Object, mapper);
        var result = await subject.GetMovie("1");
        var resultObject = result.Should().BeOfType<NotFoundObjectResult>().Subject.Value;
        var id = resultObject.Should().BeOfType<string>().Subject;

        id.Should().Be("1");

        serviceMock.Verify(x => x.GetMovie("1"), Times.Once);
        serviceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMovie_Should_Return_MovieDetails()
    {
        var result = await _subject.GetMovie("1");
        var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
        var movieDetails = resultObject.Should().BeOfType<MovieViewModel>().Subject;

        movieDetails.Should().NotBeNull();
        movieDetails.Id.Should().Be(_movie.Id);

        _movieServiceMock.Verify(x => x.GetMovie("1"));
        _movieServiceMock.VerifyNoOtherCalls();
    }

    private static Mapper CreateMapper()
    {
        var profiles = new MapProfiles();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profiles));
        return new Mapper(configuration);
    }
}