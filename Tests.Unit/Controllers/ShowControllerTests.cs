using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Query;
using EmbyStat.Controllers;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Controllers.Show;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.DataGrid;
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
            var showPage = new Page<Show>(new []
            {
                new ShowBuilder("1").Build()
            });

            _show = new ShowBuilder("1").Build();

            _showServiceMock = new Mock<IShowService>();
            _showServiceMock.Setup(x => x.GetShowLibraries()).ReturnsAsync(_collections);
            _showServiceMock.Setup(x => x.TypeIsPresent()).Returns(true);
            _showServiceMock
                .Setup(x => x.GetStatistics())
                .ReturnsAsync(statistics);
            _showServiceMock
                .Setup(x => x.GetShowPage(It.IsAny<int>(), It.IsAny<int>(), 
                        It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Filter[]>(), 
                        It.IsAny<bool>()))
                .ReturnsAsync(showPage);
            _showServiceMock.Setup(x => x.GetShow(It.IsAny<string>())).ReturnsAsync(_show);

            _subject = new ShowController(_showServiceMock.Object, mapper);
        }

        [Fact]
        public async Task Are_Show_Collections_Returned()
        {
            var result = await _subject.GetLibraries();
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
            var list = resultObject.Should().BeOfType<List<LibraryViewModel>>().Subject;

            list.Count.Should().Be(2);
            list[0].Name.Should().Be(_collections[0].Name);
            list[1].Name.Should().Be(_collections[1].Name);
            _showServiceMock.Verify(x => x.GetShowLibraries(), Times.Once);
            _showServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Should_Return_Statistics()
        {
            var result = await _subject.GetStatistics();
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
            var statistics = resultObject.Should().BeOfType<ShowStatisticsViewModel>().Subject;

            statistics.Should().NotBeNull();
            _showServiceMock.Verify(x => x.GetStatistics(), Times.Once);
            _showServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetShow_Should_Return_NotFound()
        {
            var mapper = CreateMapper();
            var serviceMock = new Mock<IShowService>();
            serviceMock.Setup(x => x.GetShow(It.IsAny<string>())).ReturnsAsync((Show) null);

            var subject = new ShowController(serviceMock.Object, mapper);
            var result = await subject.GetShow("1");
            var resultObject = result.Should().BeOfType<NotFoundObjectResult>().Subject.Value;
            var id = resultObject.Should().BeOfType<string>().Subject;

            id.Should().Be("1");
            
            serviceMock.Verify(x => x.GetShow("1"), Times.Once);
            serviceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetShow_Should_Return_ShowDetails()
        {
            var result = await _subject.GetShow("1");
            var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
            var showDetails = resultObject.Should().BeOfType<ShowDetailViewModel>().Subject;

            showDetails.Should().NotBeNull();
            showDetails.Id.Should().Be(_show.Id);
            showDetails.CumulativeRunTime.Should().Be(32334L);
            showDetails.Banner.Should().Be(_show.Banner);
            showDetails.Primary.Should().Be(_show.Primary);
            showDetails.SizeInMb.Should().Be(_show.SizeInMb);
            showDetails.Tmdb.Should().Be(_show.TMDB);
            showDetails.Name.Should().Be(_show.Name);
            showDetails.PremiereDate.Should().Be(_show.PremiereDate);
            showDetails.CommunityRating.Should().Be(_show.CommunityRating);
            showDetails.Genres.Length.Should().Be(_show.Genres.Count);
            showDetails.Imdb.Should().Be(_show.IMDB);
            showDetails.Tvdb.Should().Be(_show.TVDB);
            showDetails.Logo.Should().Be(_show.Logo);
            showDetails.Thumb.Should().Be(_show.Thumb);
            showDetails.Path.Should().Be(_show.Path);
            showDetails.ProductionYear.Should().Be(_show.ProductionYear);
            showDetails.RunTime.Should().Be(2L);
            showDetails.SpecialEpisodeCount.Should().Be(2);
            showDetails.Status.Should().Be(_show.Status);
            showDetails.SeasonCount.Should().Be(_show.Seasons.Count);
            
            _showServiceMock.Verify(x => x.GetShow("1"), Times.Once);
            _showServiceMock.VerifyNoOtherCalls();
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