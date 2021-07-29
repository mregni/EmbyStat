using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Controllers;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Controllers.Show;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Show;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tests.Unit.Builders;
using Xunit;

namespace Tests.Unit.Controllers
{
    public class ShowControllerTests
    {
        private readonly ShowController _subject;
        private readonly Mock<IShowService> _showServiceMock;
        private readonly List<Library> _collections;
        private readonly Show _show;

        public ShowControllerTests()
        {
            var mapper = CreateMapper();

            _collections = new List<Library>
            {
                new LibraryBuilder(1, LibraryType.TvShow).Build(),
                new LibraryBuilder(2, LibraryType.TvShow).Build()
            };

            var statistics = new ShowStatistics();
            var showPage = new ListContainer<ShowCollectionRow>()
            {
                TotalCount = 1,
                Data = new List<ShowCollectionRow>
                {
                    new ShowCollectionRow()
                }
            };

            _show = new ShowBuilder("1", "1")
                .AddMissingEpisodes(3, 1)
                .AddSpecialEpisode("1")
                .Build();

            _showServiceMock = new Mock<IShowService>();
            _showServiceMock.Setup(x => x.GetShowLibraries()).Returns(_collections);
            _showServiceMock.Setup(x => x.TypeIsPresent()).Returns(true);
            _showServiceMock
                .Setup(x => x.GetStatistics(It.IsAny<List<string>>()))
                .Returns(statistics);
            _showServiceMock
                .Setup(x => x.GetCollectedRows(It.IsAny<List<string>>(), It.IsAny<int>()))
                .Returns(showPage);
            _showServiceMock.Setup(x => x.GetShow(It.IsAny<string>())).Returns(_show);

            _subject = new ShowController(_showServiceMock.Object, mapper);
        }

        [Fact]
        public void Are_Show_Collections_Returned()
        {
            var result = _subject.GetLibraries();
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
            var list = resultObject.Should().BeOfType<List<LibraryViewModel>>().Subject;

            list.Count.Should().Be(2);
            list[0].Name.Should().Be(_collections[0].Name);
            list[1].Name.Should().Be(_collections[1].Name);
            _showServiceMock.Verify(x => x.GetShowLibraries(), Times.Once);
        }

        [Fact]
        public void Should_Return_Statistics()
        {
            var collectionIds = _collections.Select(x => x.Id).ToList();
            var result = _subject.GetStatistics(collectionIds);
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
            var statistics = resultObject.Should().BeOfType<ShowStatisticsViewModel>().Subject;

            statistics.Should().NotBeNull();
            _showServiceMock.Verify(x => x.GetStatistics(collectionIds), Times.Once);
        }

        [Fact]
        public void GetCollectedRows_Should_Return_Show_Page()
        {
            var collectionIds = _collections.Select(x => x.Id).ToList();
            var result = _subject.GetCollectedRows(collectionIds, 0);
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
            var page = resultObject.Should().BeOfType<ListContainer<ShowCollectionRowViewModel>>().Subject;

            page.Should().NotBeNull();
            page.TotalCount.Should().Be(1);
            page.Data.Count().Should().Be(1);
        }

        [Fact]
        public void GetShow_Should_Return_NotFound()
        {
            var mapper = CreateMapper();
            var serviceMock = new Mock<IShowService>();
            serviceMock.Setup(x => x.GetShow(It.IsAny<string>())).Returns((Show)null);

            var subject = new ShowController(serviceMock.Object, mapper);
            var result = subject.GetShow("1");
            var resultObject = result.Should().BeOfType<NotFoundObjectResult>().Subject.Value;
            var id = resultObject.Should().BeOfType<string>().Subject;

            id.Should().Be("1");
        }

        [Fact]
        public void GetShow_Should_Return_ShowDetails()
        {
            var result = _subject.GetShow("1");
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
            var showDetails = resultObject.Should().BeOfType<ShowDetailViewModel>().Subject;

            showDetails.Should().NotBeNull();
            showDetails.Id.Should().Be(_show.Id);
            showDetails.CumulativeRunTimeTicks.Should().Be(_show.CumulativeRunTimeTicks);
            showDetails.Banner.Should().Be(_show.Banner);
            showDetails.Primary.Should().Be(_show.Primary);
            showDetails.SizeInMb.Should().Be(303);
            showDetails.TMDB.Should().Be(_show.TMDB);
            showDetails.Name.Should().Be(_show.Name);
            showDetails.PremiereDate.Should().Be(_show.PremiereDate);
            showDetails.CollectedEpisodeCount.Should().Be(2);
            showDetails.CommunityRating.Should().Be(_show.CommunityRating);
            showDetails.Genres.Length.Should().Be(_show.Genres.Length);
            showDetails.IMDB.Should().Be(_show.IMDB);
            showDetails.TVDB.Should().Be(_show.TVDB);
            showDetails.Logo.Should().Be(_show.Logo);
            showDetails.Thumb.Should().Be(_show.Thumb);
            showDetails.Path.Should().Be(_show.Path);
            showDetails.ProductionYear.Should().Be(_show.ProductionYear);
            showDetails.RunTimeTicks.Should().Be(_show.RunTimeTicks);
            showDetails.SpecialEpisodeCount.Should().Be(1);
            showDetails.Status.Should().Be(_show.Status);
            showDetails.MissingEpisodes.Count.Should().Be(3);
            showDetails.SeasonCount.Should().Be(_show.Seasons.Count - 1);
        }

        [Fact]
        public void Is_Show_Type_Present()
        {
            var result = _subject.ShowTypeIsPresent();
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
            var isPresent = resultObject.Should().BeOfType<bool>().Subject;

            isPresent.Should().BeTrue();
            _showServiceMock.Verify(x => x.TypeIsPresent(), Times.Once);
        }

        private Mapper CreateMapper()
        {
            var profiles = new MapProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profiles));
            return new Mapper(configuration);
        }
    }
}
