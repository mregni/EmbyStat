using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Controllers.Movie;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Movie;
using EmbyStat.Services.Models.Stat;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.Unit.Controllers
{
    [Collection("Mapper collection")]
    public class MovieControllerTests
    {
        private readonly MovieController _subject;
        private readonly Mock<IMovieService> _movieServiceMock;
        private readonly List<Collection> _collections;
        private readonly  MovieStats _movieStats;

        public MovieControllerTests()
        {
            _collections = new List<Collection>
            {
                new Collection{ Id = "id1", Name = "collection1", PrimaryImage = "image1", Type = CollectionType.Movies},
                new Collection{ Id = "id2", Name = "collection2", PrimaryImage = "image2", Type = CollectionType.Movies}
            };

            _movieStats = new MovieStats
            {
                LongestMovie = new MoviePoster { Name = "The lord of the rings"}
            };

            _movieServiceMock = new Mock<IMovieService>();
            _movieServiceMock.Setup(x => x.GetMovieCollections()).Returns(_collections);
            _movieServiceMock.Setup(x => x.GetGeneralStatsForCollections(It.IsAny<List<string>>()))
                .Returns(_movieStats);

            var _mapperMock = new Mock<IMapper>();
            _mapperMock.Setup(x => x.Map<MovieStatsViewModel>(It.IsAny<MovieStats>())).Returns(new MovieStatsViewModel {LongestMovie = new MoviePosterViewModel { Name = "The lord of the rings" } });
            _mapperMock.Setup(x => x.Map<IList<CollectionViewModel>>(It.IsAny<List<Collection>>())).Returns(
                new List<CollectionViewModel>
                {
                    new CollectionViewModel
                    {
                        Name = "collection1",
                        PrimaryImage = "image1",
                        Type = (int)CollectionType.Movies
                    },
                    new CollectionViewModel
                    {
                        Name = "collection2",
                        PrimaryImage = "image2",
                        Type = (int)CollectionType.Movies
                    }
                });
            _subject = new MovieController(_movieServiceMock.Object, _mapperMock.Object);
        }

        public void Dispose()
        {
            _subject?.Dispose();
        }

        [Fact]
        public void AreMovieCollectionsReturned()
        {
            var result = _subject.GetCollections();
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
            var list = resultObject.Should().BeOfType<List<CollectionViewModel>>().Subject;

            list.Count.Should().Be(2);
            list[0].Name.Should().Be(_collections[0].Name);
            list[1].Name.Should().Be(_collections[1].Name);
            _movieServiceMock.Verify(x => x.GetMovieCollections(), Times.Once);
        }

        [Fact]
        public void AreMovieStatsReturned()
        {
            var result = _subject.GetGeneralStats(_collections.Select(x => x.Id).ToList());
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
            var stat = resultObject.Should().BeOfType<MovieStatsViewModel>().Subject;

            stat.Should().NotBeNull();
            stat.LongestMovie.Name.Should().Be(_movieStats.LongestMovie.Name);
            _movieServiceMock.Verify(x => x.GetGeneralStatsForCollections(It.Is<List<string>>(
                y => y[0] == _collections[0].Id &&
                     y[1] == _collections[1].Id)), Times.Once);
        }
    }
}
