using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Controllers.Movie;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Cards;
using EmbyStat.Services.Models.Movie;
using EmbyStat.Services.Models.Stat;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
                new Library{ Id = "id1", Name = "collection1", PrimaryImage = "image1", Type = LibraryType.Movies},
                new Library{ Id = "id2", Name = "collection2", PrimaryImage = "image2", Type = LibraryType.Movies}
            };

            _movieCards = new List<TopCard>
            {
                new TopCard { Title = "The lord of the rings" }
            };

            var movieStatistics = new MovieStatistics
            {
                TopCards = _movieCards
            };

            _movieServiceMock = new Mock<IMovieService>();
            _movieServiceMock.Setup(x => x.GetMovieLibraries()).Returns(_collections);
            _movieServiceMock.Setup(x => x.GetStatistics(It.IsAny<List<string>>()))
                .Returns(movieStatistics);

            var _mapperMock = new Mock<IMapper>();
            _mapperMock.Setup(x => x.Map<MovieStatisticsViewModel>(It.IsAny<MovieStatistics>()))
                .Returns(new MovieStatisticsViewModel { TopCards = new List<TopCardViewModel> { new TopCardViewModel { Title = "The lord of the rings" } } });
            _mapperMock.Setup(x => x.Map<IList<LibraryViewModel>>(It.IsAny<List<Library>>())).Returns(
                new List<LibraryViewModel>
                {
                    new LibraryViewModel
                    {
                        Name = "collection1",
                        PrimaryImage = "image1",
                        Type = (int) LibraryType.Movies
                    },
                    new LibraryViewModel
                    {
                        Name = "collection2",
                        PrimaryImage = "image2",
                        Type = (int) LibraryType.Movies
                    }
                });
            _subject = new MovieController(_movieServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void AreMovieCollectionsReturned()
        {
            var result = _subject.GetLibraries();
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
            var list = resultObject.Should().BeOfType<List<LibraryViewModel>>().Subject;

            list.Count.Should().Be(2);
            list[0].Name.Should().Be(_collections[0].Name);
            list[1].Name.Should().Be(_collections[1].Name);
            _movieServiceMock.Verify(x => x.GetMovieLibraries(), Times.Once);
        }

        [Fact]
        public void AreMovieStatsReturned()
        {
            var result = _subject.GetGeneralStats(_collections.Select(x => x.Id).ToList());
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
            var stat = resultObject.Should().BeOfType<MovieStatisticsViewModel>().Subject;

            stat.Should().NotBeNull();
            stat.TopCards.Count.Should().Be(1);
            stat.TopCards[0].Title.Should().Be(_movieCards[0].Title);
            _movieServiceMock.Verify(x => x.GetStatistics(It.Is<List<string>>(
                y => y[0] == _collections[0].Id &&
                     y[1] == _collections[1].Id)), Times.Once);
        }
    }
}
