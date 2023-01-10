using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Charts;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Query;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Jobs.Interfaces;
using EmbyStat.Core.MediaServers.Interfaces;
using EmbyStat.Core.Shows;
using EmbyStat.Core.Shows.Interfaces;
using EmbyStat.Core.Statistics.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Unit.Builders;
using Xunit;
using ValueType = EmbyStat.Common.Models.Cards.ValueType;

namespace Tests.Unit.Services;

public class ShowServiceTests
{
    private readonly ShowService _subject;

    private readonly Show _showOne;

    private readonly Mock<IShowRepository> _showRepositoryMock;
    private readonly Mock<IMediaServerRepository> _mediaServerRepositoryMock;

    public ShowServiceTests()
    {
        _showRepositoryMock = new Mock<IShowRepository>();
        _mediaServerRepositoryMock = new Mock<IMediaServerRepository>();

        var showOneId = Guid.NewGuid().ToString();
        var showTwoId = Guid.NewGuid().ToString();
        var showThreeId = Guid.NewGuid().ToString();

        _showOne = new ShowBuilder(showOneId)
            .AddName("Chuck")
            .AddCreateDate(new DateTime(1990, 4, 2))
            .AddGenre("Comedy", "Action")
            .AddCommunityRating(null)
            .Build();
        var showTwo = new ShowBuilder(showTwoId)
            .AddName("The 100")
            .AddCommunityRating(8.3M)
            .AddPremiereDate(new DateTime(1992, 4, 1))
            .AddGenre("Drama", "Comedy", "Action")
            .SetContinuing()
            .AddOfficialRating("TV-16")
            .AddActor(_showOne.People.First().Id.ToString())
            .AddEpisode(new EpisodeBuilder(Guid.NewGuid().ToString(), "1").Build())
            .AddEpisode(new EpisodeBuilder(Guid.NewGuid().ToString(), "1").Build())
            .AddMissingEpisodes(10, 1)
            .Build();
        var showThree = new ShowBuilder(showThreeId)
            .AddName("Dexter")
            .AddCommunityRating(8.4M)
            .AddPremiereDate(new DateTime(2018, 4, 10))
            .AddCreateDate(new DateTime(2003, 4, 2))
            .AddGenre("War", "Action")
            .SetContinuing()
            .AddEpisode(new EpisodeBuilder(Guid.NewGuid().ToString(), "1").Build())
            .AddMissingEpisodes(2, 1)
            .Build();

        _subject = CreateShowService(_showOne, showTwo, showThree);
    }

    private ShowService CreateShowService(params Show[] shows)
    {
        _showRepositoryMock
            .Setup(x => x.Any())
            .Returns(true);
        _showRepositoryMock
            .Setup(x => x.GetShowPage(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Filter[]>()))
            .ReturnsAsync(new[] {_showOne});
        _showRepositoryMock
            .Setup(x => x.Count(It.IsAny<Filter[]>()))
            .ReturnsAsync(11);
        _showRepositoryMock
            .Setup(x => x.GetShowByIdWithEpisodes(It.IsAny<string>()))
            .ReturnsAsync(_showOne);
        
        _mediaServerRepositoryMock.Setup(x => x.GetAllLibraries(It.IsAny<LibraryType>()))
            .ReturnsAsync(new List<Library>
            {
                new LibraryBuilder(0, LibraryType.TvShow).Build(),
                new LibraryBuilder(1, LibraryType.TvShow).Build(),
            });
        
        var statisticsServiceMock = new Mock<IStatisticsService>();
        return new ShowService(_showRepositoryMock.Object, _mediaServerRepositoryMock.Object, statisticsServiceMock.Object);
                
    }

    [Fact]
    public async Task GetCollectionsFromDatabase()
    {
        var collections = await _subject.GetShowLibraries();

        collections.Should().NotBeNull();
        collections.Count.Should().Be(2);
            
        _mediaServerRepositoryMock.Verify(x => x.GetAllLibraries(LibraryType.TvShow));
        _mediaServerRepositoryMock.VerifyNoOtherCalls();
            
        _showRepositoryMock.VerifyNoOtherCalls();
    }
        
    [Fact]
    public async Task GetShow_Should_Return_Show()
    {
        var show = await _subject.GetShow(_showOne.Id);

        show.Should().NotBeNull();
        show.Id.Should().Be(_showOne.Id);
            
        _showRepositoryMock.Verify(x => x.GetShowByIdWithEpisodes(_showOne.Id));
        _showRepositoryMock.VerifyNoOtherCalls();
    }
        
    [Fact]
    public async Task SetLibraryAsSynced_Should_Update_Libraries()
    {
        var list = new[] {"1", "2"};
        await _subject.SetLibraryAsSynced(list);

        _mediaServerRepositoryMock.Verify(x => x.SetLibraryAsSynced(list, LibraryType.TvShow));
        _mediaServerRepositoryMock.VerifyNoOtherCalls();
            
        _showRepositoryMock.Verify(x => x.RemoveUnwantedShows(list));
        _showRepositoryMock.VerifyNoOtherCalls();
    }
        
    [Fact]
    public async Task GetShowPage_Should_Return_Page_With_Total_Count()
    {
        var filters = Array.Empty<Filter>();
        var result = await _subject.GetShowPage(0, 1, "name", "asc", filters, true);
        result.Should().NotBeNull();

        result.TotalCount.Should().Be(11);
        var results = result.Data.ToList();
        results.Count.Should().Be(1);
        results[0].Id.Should().Be(_showOne.Id);
            
        _showRepositoryMock.Verify(x => x.GetShowPage(0, 1, "name", "asc", filters));
        _showRepositoryMock.Verify(x => x.Count(filters));
        _showRepositoryMock.VerifyNoOtherCalls();
            
        _mediaServerRepositoryMock.VerifyNoOtherCalls();
    }
        
    [Fact]
    public async Task GetShowPage_Should_Return_Page_WithoutTotal_Count()
    {
        var filters = Array.Empty<Filter>();
        var result = await _subject.GetShowPage(0, 1, "name", "asc", filters, false);
        result.Should().NotBeNull();

        result.TotalCount.Should().Be(0);
        var results = result.Data.ToList();
        results.Count.Should().Be(1);
        results[0].Id.Should().Be(_showOne.Id);
            
        _showRepositoryMock.Verify(x => x.GetShowPage(0, 1, "name", "asc", filters));
        _showRepositoryMock.VerifyNoOtherCalls();
            
        _mediaServerRepositoryMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public void TypeIsPresent_Should_Return_True()
    {
        var result = _subject.TypeIsPresent();
        result.Should().BeTrue();

        _showRepositoryMock.Verify(x => x.Any(), Times.Once);
    }

    #region People

    //TODO re-enable after show migration
    //[Fact]
    //public void GetMostFeaturedActorsPerGenre()
    //{
    //    var stat = await _subject.GetStatistics();

    //    stat.People.Should().NotBeNull();
    //    stat.People.MostFeaturedActorsPerGenreCards.Should().NotBeNull();
    //    stat.People.MostFeaturedActorsPerGenreCards.Count.Should().Be(4);
    //    stat.People.MostFeaturedActorsPerGenreCards[0].Title.Should().Be("Action");
    //    stat.People.MostFeaturedActorsPerGenreCards[1].Title.Should().Be("Comedy");
    //    stat.People.MostFeaturedActorsPerGenreCards[2].Title.Should().Be("Drama");
    //    stat.People.MostFeaturedActorsPerGenreCards[3].Title.Should().Be("War");
    //}

    #endregion
}