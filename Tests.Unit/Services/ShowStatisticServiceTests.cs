using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Enums.StatisticEnum;
using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.Charts;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.MediaServers.Interfaces;
using EmbyStat.Core.Shows;
using EmbyStat.Core.Shows.Interfaces;
using EmbyStat.Core.Statistics;
using EmbyStat.Core.Statistics.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MoreLinq;
using Newtonsoft.Json;
using Tests.Unit.Builders;
using Xunit;
using ValueType = EmbyStat.Common.Models.Cards.ValueType;

namespace Tests.Unit.Services;

public class ShowStatisticServiceTests
{
    private readonly ShowStatisticService _subject;

    private readonly Show _showOne;
    private readonly Show _showTwo;
    private readonly Show _showThree;

    private readonly Mock<IShowRepository> _showRepositoryMock;
    private readonly Mock<ILogger<ShowStatisticService>> _logger;
    private readonly Mock<IConfigurationService> _configurationServiceMock;

    public ShowStatisticServiceTests()
    {
        _showRepositoryMock = new Mock<IShowRepository>();
        _logger = new Mock<ILogger<ShowStatisticService>>();
        _configurationServiceMock = new Mock<IConfigurationService>();

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
            .AddSizeInMb(500)
            .AddActor(_showOne.People.First().Id.ToString())
            .AddEpisode(new EpisodeBuilder(Guid.NewGuid().ToString(), "1").Build())
            .AddEpisode(new EpisodeBuilder(Guid.NewGuid().ToString(), "1").Build())
            .AddMissingEpisodes(10, 1)
            .Build();
        _showThree = new ShowBuilder(showThreeId)
            .AddName("Dexter")
            .AddCommunityRating(8.4M)
            .AddSizeInMb(800)
            .AddPremiereDate(new DateTime(2018, 4, 10))
            .AddCreateDate(new DateTime(2003, 4, 2))
            .AddGenre("War", "Action")
            .SetContinuing()
            .AddEpisode(new EpisodeBuilder(Guid.NewGuid().ToString(), "1").Build())
            .AddMissingEpisodes(2, 1)
            .Build();
    }

    private ShowStatisticService CreateService(Action registrations)
    {
        registrations();
        return new ShowStatisticService(_logger.Object, _configurationServiceMock.Object, _showRepositoryMock.Object);
    }

    [Fact]
    public async Task CalculateTotalMovieCount_Should_Calculate()
    {
        void Registrations()
        {
            var shows = new[] {_showOne, _showTwo, _showThree};
            _showRepositoryMock
                .Setup(x => x.Count())
                .ReturnsAsync(shows.Length);
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.ShowTotalCount);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.TheatersRoundedIcon);
        obj.Title.Should().Be(Constants.Shows.TotalShows);
        obj.Type.Should().Be(CardType.Text);
        obj.Value.Should().Be("3");

        _showRepositoryMock.Verify(x => x.Count(), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateTotalShowGenres_Should_Calculate()
    {
        void Registrations()
        {
            var shows = new[] {_showOne, _showTwo, _showThree};
            _showRepositoryMock
                .Setup(x => x.GetGenreCount())
                .ReturnsAsync(shows.SelectMany(x => x.Genres).Select(x => x.Name).Distinct().Count());
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.ShowTotalGenreCount);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.PoundRoundedIcon);
        obj.Title.Should().Be(Constants.Common.TotalGenres);
        obj.Type.Should().Be(CardType.Text);
        obj.Value.Should().Be("4");

        _showRepositoryMock.Verify(x => x.GetGenreCount(), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateCompleteCollectedShowCount_Should_Calculate()
    {
        void Registrations()
        {
            var shows = new[] {_showOne, _showTwo};
            _showRepositoryMock
                .Setup(x => x.CompleteCollectedCount())
                .ReturnsAsync(shows.Length);
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.ShowCompleteCollectedCount);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.TheatersRoundedIcon);
        obj.Title.Should().Be(Constants.Shows.TotalCompleteCollectedShows);
        obj.Type.Should().Be(CardType.Text);
        obj.Value.Should().Be("2");

        _showRepositoryMock.Verify(x => x.CompleteCollectedCount(), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateTotalEpisodeCount_Should_Calculate()
    {
        void Registrations()
        {
            var shows = new[] {_showOne, _showTwo};
            _showRepositoryMock
                .Setup(x => x.GetEpisodeCount(LocationType.Disk))
                .ReturnsAsync(shows.SelectMany(x => x.Seasons).SelectMany(x => x.Episodes)
                    .Count(x => x.LocationType == LocationType.Disk));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.ShowTotalEpisodesCount);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.TheatersRoundedIcon);
        obj.Title.Should().Be(Constants.Shows.TotalEpisodes);
        obj.Type.Should().Be(CardType.Text);
        obj.Value.Should().Be("12");

        _showRepositoryMock.Verify(x => x.GetEpisodeCount(LocationType.Disk), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateTotalMissingEpisodeCount_Should_Calculate()
    {
        void Registrations()
        {
            var shows = new[] {_showOne, _showTwo};
            _showRepositoryMock
                .Setup(x => x.GetEpisodeCount(LocationType.Virtual))
                .ReturnsAsync(shows.SelectMany(x => x.Seasons).SelectMany(x => x.Episodes)
                    .Count(x => x.LocationType == LocationType.Virtual));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.ShowTotalMissingEpisodeCount);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.TheatersRoundedIcon);
        obj.Title.Should().Be(Constants.Shows.TotalMissingEpisodes);
        obj.Type.Should().Be(CardType.Text);
        obj.Value.Should().Be("10");

        _showRepositoryMock.Verify(x => x.GetEpisodeCount(LocationType.Virtual), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculatePlayableTime_Should_Calculate()
    {
        void Registrations()
        {
            var shows = new[] {_showOne, _showTwo};
            _showRepositoryMock
                .Setup(x => x.GetTotalRunTimeTicks())
                .ReturnsAsync(shows.Sum(x => x.CumulativeRunTimeTicks ?? 0));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.ShowTotalPlayLength);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.QueryBuilderRoundedIcon);
        obj.Title.Should().Be(Constants.Shows.TotalPlayLength);
        obj.Type.Should().Be(CardType.Time);
        obj.Value.Should().Be("44|21|47");

        _showRepositoryMock.Verify(x => x.GetTotalRunTimeTicks(), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateTotalDiskSpace_Should_Calculate()
    {
        void Registrations()
        {
            var shows = new[] {_showOne, _showTwo};
            _showRepositoryMock
                .Setup(x => x.GetTotalDiskSpaceUsed())
                .ReturnsAsync(shows.Sum(x => x.CumulativeRunTimeTicks ?? 0));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.ShowTotalDiskSpaceUsage);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.StorageRoundedIcon);
        obj.Title.Should().Be(Constants.Common.TotalDiskSpace);
        obj.Type.Should().Be(CardType.Size);
        obj.Value.Should().Be("38800574800000");

        _showRepositoryMock.Verify(x => x.GetTotalDiskSpaceUsed(), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateWatchedEpisodeCount_Should_Calculate()
    {
        void Registrations()
        {
            _showRepositoryMock
                .Setup(x => x.GetTotalWatchedEpisodeCount())
                .ReturnsAsync(5);
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.ShowTotalWatchedEpisodeCount);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.PoundRoundedIcon);
        obj.Title.Should().Be(Constants.Shows.TotalWatchedEpisodes);
        obj.Type.Should().Be(CardType.Text);
        obj.Value.Should().Be("5");

        _showRepositoryMock.Verify(x => x.GetTotalWatchedEpisodeCount(), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateTotalWatchedTime_Should_Calculate()
    {
        void Registrations()
        {
            var shows = new[] {_showOne, _showTwo};
            _showRepositoryMock
                .Setup(x => x.GetPlayedRuntime())
                .ReturnsAsync(shows.Sum(x => x.CumulativeRunTimeTicks ?? 0));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.ShowTotalWatchedTime);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.QueryBuilderRoundedIcon);
        obj.Title.Should().Be(Constants.Shows.PlayedPlayLength);
        obj.Type.Should().Be(CardType.Time);
        obj.Value.Should().Be("44|21|47");

        _showRepositoryMock.Verify(x => x.GetPlayedRuntime(), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }


    [Fact]
    public async Task TotalPersonTypeCount_Should_Calculate_Actors()
    {
        void Registrations()
        {
            _showRepositoryMock
                .Setup(x => x.GetPeopleCount(PersonType.Actor))
                .ReturnsAsync(2);
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.ShowTotalActorCount);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.PeopleAltRoundedIcon);
        obj.Title.Should().Be(Constants.Common.TotalActors);
        obj.Type.Should().Be(CardType.Text);
        obj.Value.Should().Be("2");

        _showRepositoryMock.Verify(x => x.GetPeopleCount(PersonType.Actor), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task CalculateTotalCurrentWatchingCount_Should_Calculate()
    {
        void Registrations()
        {
            _showRepositoryMock
                .Setup(x => x.GetCurrentWatchingCount())
                .ReturnsAsync(2);
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.ShowTotalCurrentWatchingCount);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.PlayRoundedIcon);
        obj.Title.Should().Be(Constants.Shows.CurrentPlayingCount);
        obj.Type.Should().Be(CardType.Text);
        obj.Value.Should().Be("2");

        _showRepositoryMock.Verify(x => x.GetCurrentWatchingCount(), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateNewestPremieredShow_Should_Calculate()
    {
        void Registrations()
        {
            var shows = new[] {_showOne, _showTwo, _showThree};
            _showRepositoryMock
                .Setup(x => x.GetNewestPremieredMedia(5))
                .ReturnsAsync(shows.OrderByDescending(x => x.PremiereDate));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.TopCard, Statistic.ShowNewestPremieredList);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<TopCard>(result);
        obj.Should().NotBeNull();
        obj.Title.Should().Be(Constants.Shows.NewestPremiered);
        obj.Unit.Should().Be("COMMON.DATE");
        obj.Values[0].Value.Should().Be(_showThree.PremiereDate?.ToString("s"));
        obj.Values[0].Label.Should().Be(_showThree.Name);
        obj.ValueType.Should().Be(ValueType.Date);
        obj.UnitNeedsTranslation.Should().BeTrue();

        _showRepositoryMock.Verify(x => x.GetNewestPremieredMedia(5), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateOldestPremieredShow_Should_Calculate()
    {
        void Registrations()
        {
            var shows = new[] {_showOne, _showTwo, _showThree};
            _showRepositoryMock
                .Setup(x => x.GetOldestPremieredMedia(5))
                .ReturnsAsync(shows.OrderBy(x => x.PremiereDate));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.TopCard, Statistic.ShowOldestPremieredList);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<TopCard>(result);
        obj.Should().NotBeNull();
        obj.Title.Should().Be(Constants.Shows.OldestPremiered);
        obj.Unit.Should().Be("COMMON.DATE");
        obj.Values[0].Value.Should().Be(_showTwo.PremiereDate?.ToString("s"));
        obj.Values[0].Label.Should().Be(_showTwo.Name);
        obj.ValueType.Should().Be(ValueType.Date);
        obj.UnitNeedsTranslation.Should().BeTrue();

        _showRepositoryMock.Verify(x => x.GetOldestPremieredMedia(5), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateLatestAddedShow_Should_Calculate()
    {
        void Registrations()
        {
            var shows = new[] {_showOne, _showTwo, _showThree};
            _showRepositoryMock
                .Setup(x => x.GetLatestAddedShows(5))
                .ReturnsAsync(shows.OrderByDescending(x => x.DateCreated).ToList());
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.TopCard, Statistic.ShowLatestAddedList);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<TopCard>(result);
        obj.Should().NotBeNull();
        obj.Title.Should().Be(Constants.Shows.LatestAdded);
        obj.Unit.Should().Be("COMMON.DATE");
        obj.Values[0].Value.Should().Be(_showThree.DateCreated?.ToString("s"));
        obj.Values[0].Label.Should().Be(_showThree.Name);
        obj.UnitNeedsTranslation.Should().BeTrue();
        obj.ValueType.Should().Be(ValueType.Date);

        _showRepositoryMock.Verify(x => x.GetLatestAddedShows(5), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateHighestRatedShow_Should_Calculate()
    {
        void Registrations()
        {
            var shows = new[] {_showOne, _showTwo, _showThree};
            _showRepositoryMock
                .Setup(x => x.GetHighestRatedMedia(5))
                .ReturnsAsync(shows.OrderByDescending(x => x.CommunityRating));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.TopCard, Statistic.ShowHighestRatedList);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<TopCard>(result);
        obj.Should().NotBeNull();
        obj.Title.Should().Be(Constants.Shows.HighestRatedShow);
        obj.Unit.Should().Be("/10");
        obj.Values[0].Value.Should().Be(_showThree.CommunityRating.ToString());
        obj.Values[0].Label.Should().Be(_showThree.Name);
        obj.UnitNeedsTranslation.Should().BeFalse();
        obj.ValueType.Should().Be(ValueType.None);

        _showRepositoryMock.Verify(x => x.GetHighestRatedMedia(5), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateLowestRatedShow_Should_Calculate()
    {
        void Registrations()
        {
            var shows = new[] {_showOne, _showTwo, _showThree};
            _showRepositoryMock
                .Setup(x => x.GetLowestRatedMedia(5))
                .ReturnsAsync(shows.Where(x => x.CommunityRating != null).OrderBy(x => x.CommunityRating));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.TopCard, Statistic.ShowLowestRatedList);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<TopCard>(result);
        obj.Should().NotBeNull();
        obj.Title.Should().Be(Constants.Shows.LowestRatedShow);
        obj.Unit.Should().Be("/10");
        obj.Values[0].Value.Should().Be(_showTwo.CommunityRating.ToString());
        obj.Values[0].Label.Should().Be(_showTwo.Name);
        obj.UnitNeedsTranslation.Should().BeFalse();
        obj.ValueType.Should().Be(ValueType.None);

        _showRepositoryMock.Verify(x => x.GetLowestRatedMedia(5), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateShowWithMostEpisodes_Should_Calculate()
    {
        void Registrations()
        {
            var shows = new[] {_showOne, _showTwo, _showThree};
            _showRepositoryMock
                .Setup(x => x.GetShowsWithMostEpisodes(5))
                .ReturnsAsync(shows
                    .OrderByDescending(x => x.Seasons.SelectMany(y => y.Episodes).Count())
                    .ToDictionary(x => x, x => x.Seasons.SelectMany(y => y.Episodes).Count()));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.TopCard, Statistic.ShowWithMostEpisodesList);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<TopCard>(result);
        obj.Should().NotBeNull();
        obj.Title.Should().Be(Constants.Shows.MostEpisodes);
        obj.Unit.Should().Be("#");
        obj.Values[0].Value.Should().Be(_showTwo.Seasons.SelectMany(x => x.Episodes).Count().ToString());
        obj.Values[0].Label.Should().Be(_showTwo.Name);
        obj.UnitNeedsTranslation.Should().BeFalse();
        obj.ValueType.Should().Be(ValueType.None);

        _showRepositoryMock.Verify(x => x.GetShowsWithMostEpisodes(5), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateMostDiskSpaceUsedShow_Should_Calculate()
    {
        void Registrations()
        {
            var shows = new[] {_showOne, _showTwo, _showThree};
            _showRepositoryMock
                .Setup(x => x.GetShowsWithMostDiskSpaceUsed(5))
                .ReturnsAsync(shows.OrderByDescending(x => x.SizeInMb).ToList());
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.TopCard, Statistic.ShowMostDiskUsageList);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<TopCard>(result);
        obj.Should().NotBeNull();
        obj.Title.Should().Be(Constants.Shows.MostDiskSpace);
        obj.Unit.Should().Be("#");
        obj.Values[0].Value.Should().Be(_showThree.SizeInMb.ToString(CultureInfo.CurrentCulture));
        obj.Values[0].Label.Should().Be(_showThree.Name);
        obj.UnitNeedsTranslation.Should().BeFalse();
        obj.ValueType.Should().Be(ValueType.SizeInMb);

        _showRepositoryMock.Verify(x => x.GetShowsWithMostDiskSpaceUsed(5), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateMostWatchedShows_Should_Calculate()
    {
        void Registrations()
        {
            var shows = new[] {_showOne, _showTwo, _showThree};
            _showRepositoryMock
                .Setup(x => x.GetMostWatchedShows(5))
                .ReturnsAsync(shows.ToDictionary(x => x, x => x.Genres.Count));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.TopCard, Statistic.ShowMostWatchedList);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<TopCard>(result);
        obj.Should().NotBeNull();
        obj.Title.Should().Be(Constants.Shows.MostWatchedShows);
        obj.Unit.Should().Be("#");
        obj.Values[0].Value.Should().Be(_showOne.Genres.Count.ToString());
        obj.Values[0].Label.Should().Be(_showOne.Name);
        obj.UnitNeedsTranslation.Should().BeFalse();
        obj.ValueType.Should().Be(ValueType.None);

        _showRepositoryMock.Verify(x => x.GetMostWatchedShows(5), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateGenreChart_Should_Calculate()
    {
        void Registrations()
        {
            var shows = new[] {_showOne, _showTwo, _showThree};
            _showRepositoryMock
                .Setup(x => x.GetGenreChartValues())
                .ReturnsAsync(
                    shows
                        .SelectMany(x => x.Genres)
                        .GroupBy(x => x.Name)
                        .OrderBy(x => x.Key)
                        .ToDictionary(x => x.Key, x => x.Count())
                );
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.BarChart, Statistic.ShowGenreChart);

        result.Should().NotBeNullOrWhiteSpace();
        var graph = JsonConvert.DeserializeObject<Chart>(result);
        graph.Should().NotBeNull();
        graph.Title.Should().Be(Constants.CountPerGenre);
        graph.SeriesCount.Should().Be(1);
        graph.DataSets.Length.Should().Be(4);
        TestDataSets(graph.DataSets[0], "Action", 3);
        TestDataSets(graph.DataSets[1], "Comedy", 2);
        TestDataSets(graph.DataSets[2], "Drama", 1);
        TestDataSets(graph.DataSets[3], "War", 1);

        _showRepositoryMock.Verify(x => x.GetGenreChartValues(), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateRatingChart_Should_Calculate()
    {
        void Registrations()
        {
            var showFour = new ShowBuilder(Guid.NewGuid().ToString()).AddCommunityRating(9.3M)
                .Build();

            var shows = new[] {_showOne, _showTwo, _showThree, showFour};
            _showRepositoryMock
                .Setup(x => x.GetCommunityRatings())
                .ReturnsAsync(shows.Select(x => x.CommunityRating).ToList());
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.BarChart, Statistic.ShowRatingChart);

        result.Should().NotBeNullOrWhiteSpace();
        var graph = JsonConvert.DeserializeObject<Chart>(result);
        graph.Should().NotBeNull();
        graph.Title.Should().Be(Constants.CountPerCommunityRating);
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

        _showRepositoryMock.Verify(x => x.GetCommunityRatings(), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculatePremiereYearChart_Should_Calculate()
    {
        void Registrations()
        {
            var showFour = new ShowBuilder(Guid.NewGuid().ToString())
                .AddPremiereDate(new DateTime(2002, 1, 10)).Build();
            var shows = new[] {_showOne, _showTwo, _showThree, showFour};
            _showRepositoryMock
                .Setup(x => x.GetPremiereYears())
                .ReturnsAsync(shows.Select(x => x.PremiereDate).ToList());
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.BarChart, Statistic.ShowPremiereYearChart);

        result.Should().NotBeNullOrWhiteSpace();
        var graph = JsonConvert.DeserializeObject<Chart>(result);
        graph.Should().NotBeNull();
        graph.Title.Should().Be(Constants.CountPerPremiereYear);
        graph.SeriesCount.Should().Be(1);
        graph.DataSets.Length.Should().Be(6);
        TestDataSets(graph.DataSets[0], "1990 - 1994", 1);
        TestDataSets(graph.DataSets[1], "1995 - 1999", 0);
        TestDataSets(graph.DataSets[2], "2000 - 2004", 2);
        TestDataSets(graph.DataSets[3], "2005 - 2009", 0);
        TestDataSets(graph.DataSets[4], "2010 - 2014", 0);
        TestDataSets(graph.DataSets[5], "2015 - 2019", 1);

        _showRepositoryMock.Verify(x => x.GetPremiereYears(), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateOfficialRatingChart_Should_Calculate()
    {
        void Registrations()
        {
            var shows = new[] {_showOne, _showTwo, _showThree};
            _showRepositoryMock
                .Setup(x => x.GetOfficialRatingChartValues())
                .ReturnsAsync(shows
                    .Select(x => x.OfficialRating)
                    .GroupBy(x => x)
                    .ToDictionary(x => x.Key, x => x.Count()));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.BarChart, Statistic.ShowOfficialRatingChart);

        result.Should().NotBeNullOrWhiteSpace();
        var graph = JsonConvert.DeserializeObject<Chart>(result);
        graph.Should().NotBeNull();
        graph.Title.Should().Be(Constants.CountPerOfficialRating);
        graph.SeriesCount.Should().Be(1);
        graph.DataSets.Length.Should().Be(2);
        TestDataSets(graph.DataSets[0], "R", 2);
        TestDataSets(graph.DataSets[1], "TV-16", 1);

        _showRepositoryMock.Verify(x => x.GetOfficialRatingChartValues(), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task CalculateCollectedRateChart_Should_Calculate()
    {
        void Registrations()
        {
            _showRepositoryMock
                .Setup(x => x.GetCollectedRateChart())
                .ReturnsAsync(new[] {0.04, 0.26, 0.61});
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.BarChart, Statistic.ShowCollectedRateChart);

        result.Should().NotBeNullOrWhiteSpace();
        var graph = JsonConvert.DeserializeObject<Chart>(result);
        graph.Should().NotBeNull();
        graph.Title.Should().Be(Constants.CountPerCollectedPercentage);
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

        _showRepositoryMock.Verify(x => x.GetCollectedRateChart(), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateShowStateChart_Should_Calculate()
    {
        void Registrations()
        {
            var shows = new[] {_showOne, _showTwo, _showThree};
            _showRepositoryMock
                .Setup(x => x.GetShowStatusCharValues())
                .ReturnsAsync(shows
                    .Select(x => x.Status)
                    .GroupBy(x => x)
                    .OrderBy(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.Count())
                );
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.PieChart, Statistic.ShowStateChart);

        result.Should().NotBeNullOrWhiteSpace();
        var graph = JsonConvert.DeserializeObject<Chart>(result);
        graph.Should().NotBeNull();
        graph.Title.Should().Be(Constants.Shows.ShowStatusChart);
        graph.SeriesCount.Should().Be(1);
        graph.DataSets.Length.Should().Be(2);
        TestDataSets(graph.DataSets[0], "Continuing", 2);
        TestDataSets(graph.DataSets[1], "Ended", 1);
        
        _showRepositoryMock.Verify(x => x.GetShowStatusCharValues(), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task CalculateWatchedPerDayOfWeekChart_Should_Calculate()
    {
        void Registrations()
        {
            _showRepositoryMock
                .Setup(x => x.GetWatchedPerDayOfWeekChartValues())
                .ReturnsAsync(new[]
                    {
                        new BarValue<string, int> { Serie = "user1", X = "2", Y = 2},
                        new BarValue<string, int> { Serie = "user1", X = "3", Y = 1},
                        new BarValue<string, int> { Serie = "user2", X = "3", Y = 2},
                    }
                );
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.ComplexChart, Statistic.EpisodeWatchedPerDayOfWeekChart);

        result.Should().NotBeNullOrWhiteSpace();
        var graph = JsonConvert.DeserializeObject<ComplexChart>(result);
        graph.Should().NotBeNull();
        graph.Title.Should().Be(Constants.Shows.DaysOfTheWeek);
        graph.Series.Length.Should().Be(2);
        graph.Series[0].Should().Be("user1");
        graph.Series[1].Should().Be("user2");
        graph.DataSets.Should().Be("[{\"label\":\"1\"},{\"label\":\"2\",\"user1\":2},{\"label\":\"3\",\"user1\":1,\"user2\":2},{\"label\":\"4\"},{\"label\":\"5\"},{\"label\":\"6\"},{\"label\":\"0\"}]");

        _showRepositoryMock.Verify(x => x.GetWatchedPerDayOfWeekChartValues(), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task CalculateWatchedPerHourOfDayChart_Should_Calculate()
    {
        void Registrations()
        {
            _configurationServiceMock
                .Setup(x => x.GetLocalTimeZoneInfo())
                .Returns(TimeZoneInfo.FindSystemTimeZoneById("Europe/Brussels"));

            _showRepositoryMock
                .Setup(x => x.GetWatchedPerHourOfDayChartValues())
                .ReturnsAsync(new[]
                    {
                        new BarValue<string, int> { Serie = "user1", X = "10", Y = 2},
                        new BarValue<string, int> { Serie = "user1", X = "11", Y = 1},
                        new BarValue<string, int> { Serie = "user2", X = "10", Y = 2},
                    }
                );
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.ComplexChart, Statistic.EpisodeWatchedPerHourOfDayChart);

        result.Should().NotBeNullOrWhiteSpace();
        var graph = JsonConvert.DeserializeObject<ComplexChart>(result);
        graph.Should().NotBeNull();
        graph.Title.Should().Be(Constants.Shows.WatchedPerHour);
        graph.Series.Length.Should().Be(2);
        graph.Series[0].Should().Be("user1");
        graph.Series[1].Should().Be("user2");
        graph.DataSets.Should().Be("[{\"label\":\"0001-01-01T00:00:00\"},{\"label\":\"0001-01-01T01:00:00\"},{\"label\":\"0001-01-01T02:00:00\"},{\"label\":\"0001-01-01T03:00:00\"},{\"label\":\"0001-01-01T04:00:00\"},{\"label\":\"0001-01-01T05:00:00\"},{\"label\":\"0001-01-01T06:00:00\"},{\"label\":\"0001-01-01T07:00:00\"},{\"label\":\"0001-01-01T08:00:00\"},{\"label\":\"0001-01-01T09:00:00\"},{\"label\":\"0001-01-01T10:00:00\"},{\"label\":\"0001-01-01T11:00:00\",\"user1\":2,\"user2\":2},{\"label\":\"0001-01-01T12:00:00\",\"user1\":1},{\"label\":\"0001-01-01T13:00:00\"},{\"label\":\"0001-01-01T14:00:00\"},{\"label\":\"0001-01-01T15:00:00\"},{\"label\":\"0001-01-01T16:00:00\"},{\"label\":\"0001-01-01T17:00:00\"},{\"label\":\"0001-01-01T18:00:00\"},{\"label\":\"0001-01-01T19:00:00\"},{\"label\":\"0001-01-01T20:00:00\"},{\"label\":\"0001-01-01T21:00:00\"},{\"label\":\"0001-01-01T22:00:00\"},{\"label\":\"0001-01-01T23:00:00\"}]");

        _showRepositoryMock.Verify(x => x.GetWatchedPerHourOfDayChartValues(), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.Verify(x => x.GetLocalTimeZoneInfo(), Times.Once);
        _configurationServiceMock.VerifyNoOtherCalls();
    }
    
    [Theory]
    [InlineData(StatisticCardType.Card, Statistic.ShowNewestPremieredList)]
    [InlineData(StatisticCardType.Card, Statistic.ShowOldestPremieredList)]
    [InlineData(StatisticCardType.Card, Statistic.ShowLatestAddedList)]
    [InlineData(StatisticCardType.Card, Statistic.ShowHighestRatedList)]
    [InlineData(StatisticCardType.Card, Statistic.ShowLowestRatedList)]
    [InlineData(StatisticCardType.Card, Statistic.ShowWithMostEpisodesList)]
    [InlineData(StatisticCardType.Card, Statistic.ShowMostDiskUsageList)]
    [InlineData(StatisticCardType.Card, Statistic.ShowMostWatchedList)]
    [InlineData(StatisticCardType.Card, Statistic.ShowGenreChart)]
    [InlineData(StatisticCardType.Card, Statistic.ShowRatingChart)]
    [InlineData(StatisticCardType.Card, Statistic.ShowPremiereYearChart)]
    [InlineData(StatisticCardType.Card, Statistic.ShowCollectedRateChart)]
    [InlineData(StatisticCardType.Card, Statistic.ShowOfficialRatingChart)]
    [InlineData(StatisticCardType.Card, Statistic.EpisodeWatchedPerHourOfDayChart)]
    [InlineData(StatisticCardType.Card, Statistic.EpisodeWatchedPerDayOfWeekChart)]
    [InlineData(StatisticCardType.Card, Statistic.ShowStateChart)]
    [InlineData(StatisticCardType.TopCard, Statistic.ShowTotalCount)]
    [InlineData(StatisticCardType.TopCard, Statistic.ShowTotalGenreCount)]
    [InlineData(StatisticCardType.TopCard, Statistic.ShowCompleteCollectedCount)]
    [InlineData(StatisticCardType.TopCard, Statistic.ShowTotalEpisodesCount)]
    [InlineData(StatisticCardType.TopCard, Statistic.ShowTotalMissingEpisodeCount)]
    [InlineData(StatisticCardType.TopCard, Statistic.ShowTotalPlayLength)]
    [InlineData(StatisticCardType.TopCard, Statistic.ShowTotalDiskSpaceUsage)]
    [InlineData(StatisticCardType.TopCard, Statistic.ShowTotalWatchedEpisodeCount)]
    [InlineData(StatisticCardType.TopCard, Statistic.ShowTotalWatchedTime)]
    [InlineData(StatisticCardType.TopCard, Statistic.ShowTotalActorCount)]
    [InlineData(StatisticCardType.TopCard, Statistic.ShowGenreChart)]
    [InlineData(StatisticCardType.TopCard, Statistic.ShowRatingChart)]
    [InlineData(StatisticCardType.TopCard, Statistic.ShowPremiereYearChart)]
    [InlineData(StatisticCardType.TopCard, Statistic.ShowCollectedRateChart)]
    [InlineData(StatisticCardType.TopCard, Statistic.ShowOfficialRatingChart)]
    [InlineData(StatisticCardType.TopCard, Statistic.EpisodeWatchedPerHourOfDayChart)]
    [InlineData(StatisticCardType.TopCard, Statistic.EpisodeWatchedPerDayOfWeekChart)]
    [InlineData(StatisticCardType.TopCard, Statistic.ShowStateChart)]
    [InlineData(StatisticCardType.BarChart, Statistic.ShowTotalCount)]
    [InlineData(StatisticCardType.BarChart, Statistic.ShowTotalGenreCount)]
    [InlineData(StatisticCardType.BarChart, Statistic.ShowCompleteCollectedCount)]
    [InlineData(StatisticCardType.BarChart, Statistic.ShowTotalEpisodesCount)]
    [InlineData(StatisticCardType.BarChart, Statistic.ShowTotalMissingEpisodeCount)]
    [InlineData(StatisticCardType.BarChart, Statistic.ShowTotalPlayLength)]
    [InlineData(StatisticCardType.BarChart, Statistic.ShowTotalDiskSpaceUsage)]
    [InlineData(StatisticCardType.BarChart, Statistic.ShowTotalWatchedEpisodeCount)]
    [InlineData(StatisticCardType.BarChart, Statistic.ShowTotalWatchedTime)]
    [InlineData(StatisticCardType.BarChart, Statistic.ShowTotalActorCount)]
    [InlineData(StatisticCardType.BarChart, Statistic.ShowNewestPremieredList)]
    [InlineData(StatisticCardType.BarChart, Statistic.ShowOldestPremieredList)]
    [InlineData(StatisticCardType.BarChart, Statistic.ShowLatestAddedList)]
    [InlineData(StatisticCardType.BarChart, Statistic.ShowHighestRatedList)]
    [InlineData(StatisticCardType.BarChart, Statistic.ShowLowestRatedList)]
    [InlineData(StatisticCardType.BarChart, Statistic.ShowWithMostEpisodesList)]
    [InlineData(StatisticCardType.BarChart, Statistic.ShowMostDiskUsageList)]
    [InlineData(StatisticCardType.BarChart, Statistic.ShowMostWatchedList)]
    [InlineData(StatisticCardType.BarChart, Statistic.EpisodeWatchedPerHourOfDayChart)]
    [InlineData(StatisticCardType.BarChart, Statistic.EpisodeWatchedPerDayOfWeekChart)]
    [InlineData(StatisticCardType.BarChart, Statistic.ShowStateChart)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowTotalCount)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowTotalGenreCount)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowCompleteCollectedCount)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowTotalEpisodesCount)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowTotalMissingEpisodeCount)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowTotalPlayLength)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowTotalDiskSpaceUsage)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowTotalWatchedEpisodeCount)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowTotalWatchedTime)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowTotalActorCount)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowNewestPremieredList)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowOldestPremieredList)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowLatestAddedList)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowHighestRatedList)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowLowestRatedList)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowWithMostEpisodesList)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowMostDiskUsageList)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowMostWatchedList)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowGenreChart)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowRatingChart)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowPremiereYearChart)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowCollectedRateChart)]
    [InlineData(StatisticCardType.PieChart, Statistic.ShowOfficialRatingChart)]
    [InlineData(StatisticCardType.PieChart, Statistic.EpisodeWatchedPerHourOfDayChart)]
    [InlineData(StatisticCardType.PieChart, Statistic.EpisodeWatchedPerDayOfWeekChart)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowTotalCount)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowTotalGenreCount)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowCompleteCollectedCount)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowTotalEpisodesCount)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowTotalMissingEpisodeCount)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowTotalPlayLength)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowTotalDiskSpaceUsage)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowTotalWatchedEpisodeCount)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowTotalWatchedTime)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowTotalActorCount)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowNewestPremieredList)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowOldestPremieredList)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowLatestAddedList)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowHighestRatedList)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowLowestRatedList)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowWithMostEpisodesList)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowMostDiskUsageList)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowMostWatchedList)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowGenreChart)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowRatingChart)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowPremiereYearChart)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowCollectedRateChart)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowOfficialRatingChart)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.ShowStateChart)]
    public async Task Should_Return_Null(StatisticCardType type, Statistic uniqueId)
    {
        var service = new ShowStatisticService(_logger.Object, _configurationServiceMock.Object, _showRepositoryMock.Object);
        var result = await service.CalculateStatistic(type, uniqueId);
        result.Should().BeNull();
        
        _showRepositoryMock.VerifyNoOtherCalls();
        _configurationServiceMock.VerifyNoOtherCalls();
    }
    
    private static void TestDataSets(SimpleChartData set, string label, int value)
    {
        set.Label.Should().Be(label);
        set.Value.Should().Be(value);
    }
}