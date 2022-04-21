using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Query;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using EmbyStat.Services.Models.Cards;
using EmbyStat.Services.Models.Charts;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Unit.Builders;
using Xunit;
using ValueType = EmbyStat.Services.Models.Cards.ValueType;

namespace Tests.Unit.Services;

public class ShowServiceTests
{
    private readonly ShowService _subject;

    private readonly Show _showOne;
    private readonly Show _showTwo;
    private readonly Show _showThree;

    private readonly Mock<IShowRepository> _showRepositoryMock;
    private readonly Mock<IMediaServerRepository> _mediaServerRepositoryMock;
    private readonly Mock<ILogger<ShowService>> _logger;

    public ShowServiceTests()
    {
        _showRepositoryMock = new Mock<IShowRepository>();
        _logger = new Mock<ILogger<ShowService>>();
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
        _showTwo = new ShowBuilder(showTwoId)
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
        _showThree = new ShowBuilder(showThreeId)
            .AddName("Dexter")
            .AddCommunityRating(8.4M)
            .AddPremiereDate(new DateTime(2018, 4, 10))
            .AddCreateDate(new DateTime(2003, 4, 2))
            .AddGenre("War", "Action")
            .SetContinuing()
            .AddEpisode(new EpisodeBuilder(Guid.NewGuid().ToString(), "1").Build())
            .AddMissingEpisodes(2, 1)
            .Build();

        _subject = CreateShowService(_showOne, _showTwo, _showThree);
    }

    private ShowService CreateShowService(params Show[] shows)
    {
        _showRepositoryMock
            .Setup(x => x.GetAllShowsWithEpisodes())
            .ReturnsAsync(shows.ToList());
        _showRepositoryMock
            .Setup(x => x.GetHighestRatedMedia(5))
            .ReturnsAsync(shows.OrderByDescending(x => x.CommunityRating));
        _showRepositoryMock
            .Setup(x => x.GetLowestRatedMedia(5))
            .ReturnsAsync(shows.Where(x => x.CommunityRating != null).OrderBy(x => x.CommunityRating));
        _showRepositoryMock
            .Setup(x => x.GetLatestAddedMedia(5))
            .Returns(shows.OrderByDescending(x => x.DateCreated));
        _showRepositoryMock
            .Setup(x => x.GetNewestPremieredMedia(5))
            .ReturnsAsync(shows.OrderByDescending(x => x.PremiereDate));
        _showRepositoryMock
            .Setup(x => x.GetOldestPremieredMedia(5))
            .ReturnsAsync(shows.OrderBy(x => x.PremiereDate));
        _showRepositoryMock
            .Setup(x => x.GetShowsWithMostEpisodes(5))
            .ReturnsAsync(shows
                .OrderByDescending(x => x.Seasons.SelectMany(y => y.Episodes).Count())
                .ToDictionary(x => x, x => x.Seasons.SelectMany(y => y.Episodes).Count()));
        _showRepositoryMock
            .Setup(x => x.Any())
            .Returns(true);
        _showRepositoryMock
            .Setup(x => x.GetGenreChartValues())
            .ReturnsAsync(shows
                .SelectMany(x => x.Genres)
                .GroupBy(x => x.Name)
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Count())
            );
        _showRepositoryMock
            .Setup(x => x.GetOfficialRatingChartValues())
            .ReturnsAsync(shows
                .Select(x => x.OfficialRating)
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count()));
        _showRepositoryMock
            .Setup(x => x.GetShowStatusCharValues())
            .ReturnsAsync(shows
                .Select(x => x.Status)
                .GroupBy(x => x)
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Count()));
        _showRepositoryMock
            .Setup(x => x.GetCollectedRateChart())
            .ReturnsAsync(new[] {0.04, 0.26, 0.61});
        _showRepositoryMock
            .Setup(x => x.GetPremiereYears())
            .Returns(shows.Select(x => x.PremiereDate));
        _showRepositoryMock
            .Setup(x => x.Count())
            .ReturnsAsync(shows.Length);
        _showRepositoryMock
            .Setup(x => x.GetTotalRunTimeTicks())
            .ReturnsAsync(shows.Sum(x => x.CumulativeRunTimeTicks ?? 0));
        _showRepositoryMock
            .Setup(x => x.GetEpisodeCount(LocationType.Disk))
            .ReturnsAsync(shows.SelectMany(x => x.Seasons).SelectMany(x => x.Episodes).Count(x => x.LocationType == LocationType.Disk));
        _showRepositoryMock
            .Setup(x => x.GetEpisodeCount(LocationType.Virtual))
            .ReturnsAsync(shows.SelectMany(x => x.Seasons).SelectMany(x => x.Episodes).Count(x => x.LocationType == LocationType.Virtual));
        _showRepositoryMock
            .Setup(x => x.GetTotalDiskSpaceUsed())
            .ReturnsAsync(shows
                .SelectMany(x => x.Seasons)
                .SelectMany(x => x.Episodes).Where(x => x.LocationType == LocationType.Disk)
                .Sum(x => x.MediaSources.Any() ? x.MediaSources.First().SizeInMb : 0d));
        _showRepositoryMock
            .Setup(x => x.GetCommunityRatings())
            .Returns(shows.Select(x => x.CommunityRating));
        _showRepositoryMock
            .Setup(x => x.GetShowByIdWithEpisodes(It.IsAny<string>()))
            .ReturnsAsync(_showOne);
        _showRepositoryMock
            .Setup(x => x.GetShowPage(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Filter[]>()))
            .ReturnsAsync(new[] {_showOne});
        _showRepositoryMock
            .Setup(x => x.Count(It.IsAny<Filter[]>()))
            .ReturnsAsync(11);
            
        _mediaServerRepositoryMock.Setup(x => x.GetAllLibraries(It.IsAny<LibraryType>()))
            .ReturnsAsync(new List<Library>
            {
                new LibraryBuilder(0, LibraryType.TvShow).Build(),
                new LibraryBuilder(1, LibraryType.TvShow).Build(),
            });

        var statisticsRepositoryMock = new Mock<IStatisticsRepository>();
        var jobRepositoryMock = new Mock<IJobRepository>();
        return new ShowService(jobRepositoryMock.Object, _showRepositoryMock.Object,
            statisticsRepositoryMock.Object, _mediaServerRepositoryMock.Object, _logger.Object);
                
    }

    #region General

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
    public async Task GetShowCountStat()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.Cards.Should().NotBeNull();
        stat.Cards.Count(x => x.Title == Constants.Shows.TotalShows).Should().Be(1);

        var card = stat.Cards.First(x => x.Title == Constants.Shows.TotalShows);
        card.Title.Should().Be(Constants.Shows.TotalShows);
        card.Value.Should().Be("3");
    }

    [Fact]
    public async Task GetTotalEpisodeCount()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.Cards.Should().NotBeNull();
        stat.Cards.Count(x => x.Title == Constants.Shows.TotalEpisodes).Should().Be(1);

        var card = stat.Cards.First(x => x.Title == Constants.Shows.TotalEpisodes);
        card.Title.Should().Be(Constants.Shows.TotalEpisodes);
        card.Value.Should().Be("19");
    }

    [Fact]
    public async Task GetTotalMissingEpisodeCount()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.Cards.Should().NotBeNull();
        stat.Cards.Count(x => x.Title == Constants.Shows.TotalMissingEpisodes).Should().Be(1);

        var card = stat.Cards.First(x => x.Title == Constants.Shows.TotalMissingEpisodes);
        card.Title.Should().Be(Constants.Shows.TotalMissingEpisodes);
        card.Value.Should().Be("12");
    }

    [Fact]
    public async Task GetCalculatePlayableTime()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.Cards.Should().NotBeNull();
        stat.Cards.Count(x => x.Title == Constants.Shows.TotalPlayLength).Should().Be(1);

        var card = stat.Cards.First(x => x.Title == Constants.Shows.TotalPlayLength);
        card.Title.Should().Be(Constants.Shows.TotalPlayLength);
        card.Value.Should().Be("67|8|41");
    }

    [Fact]
    public async Task GetHighestRatedShow()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.TopCards.Count(x => x.Title == Constants.Shows.HighestRatedShow).Should().Be(1);

        var card = stat.TopCards.First(x => x.Title == Constants.Shows.HighestRatedShow);
        card.Should().NotBeNull();
        card.Title.Should().Be(Constants.Shows.HighestRatedShow);
        card.Unit.Should().Be("/10");
        card.Values[0].Value.Should().Be(_showThree.CommunityRating.ToString());
        card.Values[0].Label.Should().Be(_showThree.Name);
        card.UnitNeedsTranslation.Should().Be(false);
        card.ValueType.Should().Be(ValueType.None);
    }

    [Fact]
    public async Task GetLowestRatedShow()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.TopCards.Count(x => x.Title == Constants.Shows.LowestRatedShow).Should().Be(1);

        var card = stat.TopCards.First(x => x.Title == Constants.Shows.LowestRatedShow);
        card.Should().NotBeNull();
        card.Title.Should().Be(Constants.Shows.LowestRatedShow);
        card.Unit.Should().Be("/10");
        card.Values[0].Value.Should().Be(_showTwo.CommunityRating.ToString());
        card.Values[0].Label.Should().Be(_showTwo.Name);
        card.UnitNeedsTranslation.Should().Be(false);
        card.ValueType.Should().Be(ValueType.None);
    }

    [Fact]
    public async Task GetOldestPremieredShow()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.TopCards.Count(x => x.Title == Constants.Shows.OldestPremiered).Should().Be(1);

        var card = stat.TopCards.First(x => x.Title == Constants.Shows.OldestPremiered);
        card.Should().NotBeNull();
        card.Title.Should().Be(Constants.Shows.OldestPremiered);
        card.Unit.Should().Be("COMMON.DATE");
        card.Values[0].Value.Should().Be(_showTwo.PremiereDate?.ToString("s"));
        card.Values[0].Label.Should().Be(_showTwo.Name);
        card.UnitNeedsTranslation.Should().Be(true);
        card.ValueType.Should().Be(ValueType.Date);
    }

    [Fact]
    public async Task GetShowWithMostEpisodes()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.TopCards.Count(x => x.Title == Constants.Shows.MostEpisodes).Should().Be(1);

        var card = stat.TopCards.First(x => x.Title == Constants.Shows.MostEpisodes);
        card.Should().NotBeNull();
        card.Title.Should().Be(Constants.Shows.MostEpisodes);
        card.Unit.Should().Be("#");
        card.Values[0].Value.Should().Be(_showTwo.Seasons.SelectMany(x => x.Episodes).Count().ToString());
        card.Values[0].Label.Should().Be(_showTwo.Name);
        card.UnitNeedsTranslation.Should().Be(false);
        card.ValueType.Should().Be(ValueType.None);
    }

    [Fact]
    public async Task GetLatestAddedShow()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.TopCards.Count(x => x.Title == Constants.Shows.LatestAdded).Should().Be(1);

        var card = stat.TopCards.First(x => x.Title == Constants.Shows.LatestAdded);
        card.Should().NotBeNull();
        card.Title.Should().Be(Constants.Shows.LatestAdded);
        card.Unit.Should().Be("COMMON.DATE");
        card.Values[0].Value.Should().Be(_showThree.DateCreated?.ToString("s"));
        card.Values[0].Label.Should().Be(_showThree.Name);
        card.UnitNeedsTranslation.Should().Be(true);
        card.ValueType.Should().Be(ValueType.Date);
    }

    [Fact]
    public async Task GetNewestPremieredShow()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.TopCards.Count(x => x.Title == Constants.Shows.NewestPremiered).Should().Be(1);

        var card = stat.TopCards.First(x => x.Title == Constants.Shows.NewestPremiered);
        card.Should().NotBeNull();
        card.Title.Should().Be(Constants.Shows.NewestPremiered);
        card.Unit.Should().Be("COMMON.DATE");
        card.Values[0].Value.Should().Be(_showThree.PremiereDate?.ToString("s"));
        card.Values[0].Label.Should().Be(_showThree.Name);
        card.UnitNeedsTranslation.Should().Be(true);
        card.ValueType.Should().Be(ValueType.Date);
    }

    [Fact]
    public async Task GetTotalDiskSize()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.Cards.Count(x => x.Title == Constants.Common.TotalDiskSpace).Should().Be(1);

        var card = stat.Cards.First(x => x.Title == Constants.Common.TotalDiskSpace);
        card.Should().NotBeNull();
        card.Title.Should().Be(Constants.Common.TotalDiskSpace);
        card.Value.Should().Be("1919");
    }

    #endregion

    #region Charts

    [Fact]
    public async Task GetGenreChart()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.BarCharts.Should().NotBeNull();
        stat.BarCharts.Count.Should().Be(5);
        stat.BarCharts.Any(x => x.Title == Constants.CountPerGenre).Should().BeTrue();

        var graph = stat.BarCharts.Single(x => x.Title == Constants.CountPerGenre);
        graph.Should().NotBeNull();
        graph.SeriesCount.Should().Be(1);
        graph.DataSets.Length.Should().Be(4);
        TestDataSets(graph.DataSets[0], "Action", 3);
        TestDataSets(graph.DataSets[1], "Comedy", 2);
        TestDataSets(graph.DataSets[2], "Drama", 1);
        TestDataSets(graph.DataSets[3], "War", 1);
    }

    [Fact]
    public async Task GetRatingChart()
    {
        var showFour = new ShowBuilder(Guid.NewGuid().ToString()).AddCommunityRating(9.3M)
            .Build();
        var subject = CreateShowService(_showOne, _showTwo, _showThree, showFour);

        var stat = await subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.Should().NotBeNull();
        stat.BarCharts.Count.Should().Be(5);
        stat.BarCharts.Any(x => x.Title == Constants.CountPerCommunityRating).Should().BeTrue();

        var graph = stat.BarCharts.Single(x => x.Title == Constants.CountPerCommunityRating);
        graph.Should().NotBeNull();
        graph.SeriesCount.Should().Be(1);
        graph.DataSets.Length.Should().Be(21);
        TestDataSets(graph.DataSets[0], "0", 0);
        TestDataSets(graph.DataSets[1], 0.5.ToString(CultureInfo.CurrentCulture), 0);
        TestDataSets(graph.DataSets[2], "1", 0);
        TestDataSets(graph.DataSets[3], 1.5.ToString(CultureInfo.CurrentCulture), 0);
        TestDataSets(graph.DataSets[4], "2", 0);
        TestDataSets(graph.DataSets[5], 2.5.ToString(CultureInfo.CurrentCulture), 0);
        TestDataSets(graph.DataSets[6], "3", 0);
        TestDataSets(graph.DataSets[7], 3.5.ToString(CultureInfo.CurrentCulture), 0);
        TestDataSets(graph.DataSets[8], "4", 0);
        TestDataSets(graph.DataSets[9], 4.5.ToString(CultureInfo.CurrentCulture), 0);
        TestDataSets(graph.DataSets[10], "5", 0);
        TestDataSets(graph.DataSets[11], 5.5.ToString(CultureInfo.CurrentCulture), 0);
        TestDataSets(graph.DataSets[12], "6", 0);
        TestDataSets(graph.DataSets[13], 6.5.ToString(CultureInfo.CurrentCulture), 0);
        TestDataSets(graph.DataSets[14], "7", 0);
        TestDataSets(graph.DataSets[15], 7.5.ToString(CultureInfo.CurrentCulture), 0);
        TestDataSets(graph.DataSets[16], "8", 0);
        TestDataSets(graph.DataSets[17], 8.5.ToString(CultureInfo.CurrentCulture), 2);
        TestDataSets(graph.DataSets[18], "9", 0);
        TestDataSets(graph.DataSets[19], 9.5.ToString(CultureInfo.CurrentCulture), 1);
        TestDataSets(graph.DataSets[20], "UNKNOWN", 1);
    }

    [Fact]
    public async Task GetPremiereYearChart()
    {
        var showFour = new ShowBuilder(Guid.NewGuid().ToString())
            .AddPremiereDate(new DateTime(2002, 1, 10)).Build();
        var subject = CreateShowService(_showOne, _showTwo, _showThree, showFour);

        var stat = await subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.Should().NotBeNull();
        stat.BarCharts.Count.Should().Be(5);
        stat.BarCharts.Any(x => x.Title == Constants.CountPerPremiereYear).Should().BeTrue();

        var graph = stat.BarCharts.Single(x => x.Title == Constants.CountPerPremiereYear);
        graph.Should().NotBeNull();
        graph.SeriesCount.Should().Be(1);
        graph.DataSets.Length.Should().Be(6);
        TestDataSets(graph.DataSets[0], "1990 - 1994", 1);
        TestDataSets(graph.DataSets[1], "1995 - 1999", 0);
        TestDataSets(graph.DataSets[2], "2000 - 2004", 2);
        TestDataSets(graph.DataSets[3], "2005 - 2009", 0);
        TestDataSets(graph.DataSets[4], "2010 - 2014", 0);
        TestDataSets(graph.DataSets[5], "2015 - 2019", 1);
    }

    [Fact]
    public async Task GetCollectedRateChart()
    {
        var showFour = new ShowBuilder(Guid.NewGuid().ToString()).ClearEpisodes().Build();
        var subject = CreateShowService(_showOne, _showTwo, _showThree, showFour);
        var stat = await subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.Should().NotBeNull();
        stat.BarCharts.Count.Should().Be(5);
        stat.BarCharts.Any(x => x.Title == Constants.CountPerCollectedPercentage).Should().BeTrue();

        var graph = stat.BarCharts.Single(x => x.Title == Constants.CountPerCollectedPercentage);
        graph.Should().NotBeNull();
        graph.SeriesCount.Should().Be(1);
        graph.DataSets.Length.Should().Be(20);
        TestDataSets(graph.DataSets[0], "0% - 4%", 1);
        TestDataSets(graph.DataSets[1], "5% - 9%", 0);
        TestDataSets(graph.DataSets[2], "10% - 14%", 0);
        TestDataSets(graph.DataSets[3], "15% - 19%", 0);
        TestDataSets(graph.DataSets[4], "20% - 24%", 0);
        TestDataSets(graph.DataSets[5], "25% - 29%", 1);
        TestDataSets(graph.DataSets[6], "30% - 34%", 0);
        TestDataSets(graph.DataSets[7], "35% - 39%", 0);
        TestDataSets(graph.DataSets[8], "40% - 44%", 0);
        TestDataSets(graph.DataSets[9], "45% - 49%", 0);
        TestDataSets(graph.DataSets[10], "50% - 54%", 0);
        TestDataSets(graph.DataSets[11], "55% - 59%", 0);
        TestDataSets(graph.DataSets[12], "60% - 64%", 1);
        TestDataSets(graph.DataSets[13], "65% - 69%", 0);
        TestDataSets(graph.DataSets[14], "70% - 74%", 0);
        TestDataSets(graph.DataSets[15], "75% - 79%", 0);
        TestDataSets(graph.DataSets[16], "80% - 84%", 0);
        TestDataSets(graph.DataSets[17], "85% - 89%", 0);
        TestDataSets(graph.DataSets[18], "90% - 94%", 0);
        TestDataSets(graph.DataSets[19], "95% - 99%", 0);
    }

    private void TestDataSets(SimpleChartData set, string label, int value)
    {
        set.Label.Should().Be(label);
        set.Value.Should().Be(value);
    }

    [Fact]
    public async Task GetOfficialRatingChart()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.Should().NotBeNull();
        stat.BarCharts.Count.Should().Be(5);
        stat.BarCharts.Any(x => x.Title == Constants.CountPerOfficialRating).Should().BeTrue();

        var graph = stat.BarCharts.Single(x => x.Title == Constants.CountPerOfficialRating);
        graph.Should().NotBeNull();
        graph.SeriesCount.Should().Be(1);
        graph.DataSets.Length.Should().Be(2);
        TestDataSets(graph.DataSets[0], "R", 2);
        TestDataSets(graph.DataSets[1], "TV-16", 1);
    }

    [Fact]
    public async Task GetShowStateChart()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.Should().NotBeNull();
        stat.PieCharts.Count.Should().Be(1);
        stat.PieCharts.Any(x => x.Title == Constants.Shows.ShowStatusChart).Should().BeTrue();

        var graph = stat.PieCharts.Single(x => x.Title == Constants.Shows.ShowStatusChart);
        graph.Should().NotBeNull();
        graph.SeriesCount.Should().Be(1);
        graph.DataSets.Length.Should().Be(2);
        TestDataSets(graph.DataSets[0], "Continuing", 2);
        TestDataSets(graph.DataSets[1], "Ended", 1);
    }

    #endregion

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