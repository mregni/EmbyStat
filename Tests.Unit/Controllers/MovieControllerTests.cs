using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Controllers.Movie;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Cards;
using EmbyStat.Services.Models.Movie;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tests.Unit.Builders;
using Xunit;

namespace Tests.Unit.Controllers
{
    public class MovieControllerTests
    {
        private readonly MovieController _subject;
        private readonly Mock<IMovieService> _movieServiceMock;
        private readonly List<Library> _collections;
        private readonly List<TopCard> _movieCards;

        public MovieControllerTests()
        {
            _collections = new List<Library>
            {
                new LibraryBuilder(1, LibraryType.Movies).Build(),
                new LibraryBuilder(2, LibraryType.Movies).Build()
            };

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

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<MovieStatisticsViewModel>(It.IsAny<MovieStatistics>()))
                .Returns(new MovieStatisticsViewModel
                    {TopCards = new List<TopCardViewModel> {new() {Title = "The lord of the rings"}}});
            mapperMock.Setup(x => x.Map<IList<LibraryViewModel>>(It.IsAny<List<Library>>())).Returns(
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
    }
}