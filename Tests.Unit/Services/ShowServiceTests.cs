using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Cards;
using FluentAssertions;
using Moq;
using Tests.Unit.Builders;
using Xunit;

namespace Tests.Unit.Services
{
    public class ShowServiceTests
    {
        private readonly List<Library> _collections;
        private readonly ShowService _subject;

        private readonly Show _showOne;
        private readonly Show _showTwo;
        private readonly Show _showThree;

        private readonly Mock<IShowRepository> _showRepositoryMock;

        public ShowServiceTests()
        {
            _showRepositoryMock = new Mock<IShowRepository>();

            _collections = new List<Library>
            {
                new Library{ Id = string.Empty, Name = "collection1", PrimaryImage = "image1", Type = LibraryType.TvShow},
                new Library{ Id = string.Empty, Name = "collection2", PrimaryImage = "image2", Type = LibraryType.TvShow}
            };

            var showOneId = Guid.NewGuid().ToString();
            var showTwoId = Guid.NewGuid().ToString();
            var showThreeId = Guid.NewGuid().ToString();

            _showOne = new ShowBuilder(showOneId, _collections.First().Id)
                .AddName("Chuck")
                .AddCreateDate(new DateTime(1990, 4, 2))
                .AddGenre("Comedy", "Action")
                .AddCommunityRating(null)
                .Build();
            _showTwo = new ShowBuilder(showTwoId, _collections.First().Id)
                .AddName("The 100")
                .AddMissingEpisodes(10, 0)
                .AddCommunityRating(8.3f)
                .AddPremiereDate(new DateTime(1992, 4, 1))
                .AddEpisode(new EpisodeBuilder(Guid.NewGuid().ToString(), showTwoId, "1").Build())
                .AddEpisode(new EpisodeBuilder(Guid.NewGuid().ToString(), showTwoId, "1").Build())
                .AddGenre("Drama", "Comedy", "Action")
                .SetContinuing()
                .AddOfficialRating("TV-16")
                .AddActor(_showOne.People.First().Id)
                .Build();
            _showThree = new ShowBuilder(showThreeId, _collections.First().Id)
                .AddName("Dexter")
                .AddMissingEpisodes(2, 0)
                .AddCommunityRating(8.4f)
                .AddPremiereDate(new DateTime(2018, 4, 10))
                .AddEpisode(new EpisodeBuilder(Guid.NewGuid().ToString(), showThreeId, "1").Build())
                .AddCreateDate(new DateTime(2003, 4, 2))
                .AddGenre("War", "Action")
                .SetContinuing()
                .Build();

            _subject = CreateShowService(_showOne, _showTwo, _showThree);
        }

        private ShowService CreateShowService(params Show[] shows)
        {
            _showRepositoryMock
                .Setup(x => x.GetAllShows(It.IsAny<IReadOnlyList<string>>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(shows.ToList());
            _showRepositoryMock
                .Setup(x => x.GetHighestRatedMedia(It.IsAny<IReadOnlyList<string>>(), 5))
                .Returns(shows.OrderByDescending(x => x.CommunityRating));
            _showRepositoryMock
                .Setup(x => x.GetLowestRatedMedia(It.IsAny<IReadOnlyList<string>>(), 5))
                .Returns(shows.Where(x => x.CommunityRating != null).OrderBy(x => x.CommunityRating));
            _showRepositoryMock
                .Setup(x => x.GetLatestAddedMedia(It.IsAny<IReadOnlyList<string>>(), 5))
                .Returns(shows.OrderByDescending(x => x.DateCreated));
            _showRepositoryMock.
                Setup(x => x.GetMediaCount(It.IsAny<IReadOnlyList<string>>()))
                .Returns(shows.Length);
            _showRepositoryMock
                .Setup(x => x.GetNewestPremieredMedia(It.IsAny<IReadOnlyList<string>>(), 5))
                .Returns(shows.OrderByDescending(x => x.PremiereDate));
            _showRepositoryMock
                .Setup(x => x.GetOldestPremieredMedia(It.IsAny<IReadOnlyList<string>>(), 5))
                .Returns(shows.OrderBy(x => x.PremiereDate));
            _showRepositoryMock
                .Setup(x => x.GetShowsWithMostEpisodes(It.IsAny<IReadOnlyList<string>>(), 5))
                .Returns(shows.OrderByDescending(x => x.Episodes.Count).ToDictionary(x => x, x => x.Episodes.Count));
            _showRepositoryMock
                .Setup(x => x.Any())
                .Returns(true);

            foreach (var show in shows)
            {
                _showRepositoryMock.Setup(x => x.GetAllEpisodesForShow(show.Id)).Returns(show.Episodes);
            }

            var collectionRepositoryMock = new Mock<ILibraryRepository>();
            collectionRepositoryMock.Setup(x => x.GetLibrariesById(It.IsAny<IEnumerable<string>>())).Returns(_collections);

            var personServiceMock = new Mock<IPersonService>();
            foreach (var person in shows.SelectMany(x => x.People))
            {
                personServiceMock.Setup(x => x.GetPersonByNameForShows(person.Name, It.IsAny<string>())).Returns(
                    new Person
                    {
                        Id = person.Id,
                        Name = person.Name,
                        BirthDate = new DateTime(2000, 1, 1),
                        Primary = "primary.jpg",
                        MovieCount = 0,
                        ShowCount = 0
                    });
            }

            var settingsServiceMock = new Mock<ISettingsService>();
            settingsServiceMock
                .Setup(x => x.GetUserSettings())
                .Returns(new UserSettings { ShowLibraries = new List<string> { _collections[0].Id, _collections[1].Id } });
            var statisticsRepositoryMock = new Mock<IStatisticsRepository>();
            var jobRepositoryMock = new Mock<IJobRepository>();
            return new ShowService(jobRepositoryMock.Object, _showRepositoryMock.Object, collectionRepositoryMock.Object, personServiceMock.Object, statisticsRepositoryMock.Object, settingsServiceMock.Object);
        }

        #region General

        [Fact]
        public void GetCollectionsFromDatabase()
        {
            var collections = _subject.GetShowLibraries().ToList();

            collections.Should().NotBeNull();
            collections.Count().Should().Be(2);
        }

        [Fact]
        public void GetShowCountStat()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Cards.Should().NotBeNull();
            stat.Cards.Count(x => x.Title == Constants.Shows.TotalShows).Should().Be(1);

            var card = stat.Cards.First(x => x.Title == Constants.Shows.TotalShows);
            card.Title.Should().Be(Constants.Shows.TotalShows);
            card.Value.Should().Be("3");
        }

        [Fact]
        public void GetTotalEpisodeCount()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Cards.Should().NotBeNull();
            stat.Cards.Count(x => x.Title == Constants.Shows.TotalEpisodes).Should().Be(1);

            var card = stat.Cards.First(x => x.Title == Constants.Shows.TotalEpisodes);
            card.Title.Should().Be(Constants.Shows.TotalEpisodes);
            card.Value.Should().Be("9");
        }

        [Fact]
        public void GetTotalMissingEpisodeCount()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Cards.Should().NotBeNull();
            stat.Cards.Count(x => x.Title == Constants.Shows.TotalMissingEpisodes).Should().Be(1);

            var card = stat.Cards.First(x => x.Title == Constants.Shows.TotalMissingEpisodes);
            card.Title.Should().Be(Constants.Shows.TotalMissingEpisodes);
            card.Value.Should().Be("12");
        }

        [Fact]
        public void GetCalculatePlayableTime()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Cards.Should().NotBeNull();
            stat.Cards.Count(x => x.Title == Constants.Shows.TotalPlayLength).Should().Be(1);

            var card = stat.Cards.First(x => x.Title == Constants.Shows.TotalPlayLength);
            card.Title.Should().Be(Constants.Shows.TotalPlayLength);
            card.Value.Should().Be("67|8|41");
        }

        [Fact]
        public void GetHighestRatedShow()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.TopCards.Count(x => x.Title == Constants.Shows.HighestRatedShow).Should().Be(1);

            var card = stat.TopCards.First(x => x.Title == Constants.Shows.HighestRatedShow);
            card.Should().NotBeNull();
            card.Title.Should().Be(Constants.Shows.HighestRatedShow);
            card.Unit.Should().Be("/10");
            card.Values[0].Value.Should().Be(_showThree.CommunityRating.ToString());
            card.Values[0].Label.Should().Be(_showThree.Name);
            card.UnitNeedsTranslation.Should().Be(false);
            card.ValueType.Should().Be(ValueTypeEnum.None);
        }

        [Fact]
        public void GetLowestRatedShow()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.TopCards.Count(x => x.Title == Constants.Shows.LowestRatedShow).Should().Be(1);

            var card = stat.TopCards.First(x => x.Title == Constants.Shows.LowestRatedShow);
            card.Should().NotBeNull();
            card.Title.Should().Be(Constants.Shows.LowestRatedShow);
            card.Unit.Should().Be("/10");
            card.Values[0].Value.Should().Be(_showTwo.CommunityRating.ToString());
            card.Values[0].Label.Should().Be(_showTwo.Name);
            card.UnitNeedsTranslation.Should().Be(false);
            card.ValueType.Should().Be(ValueTypeEnum.None);
        }

        [Fact]
        public void GetOldestPremieredShow()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.TopCards.Count(x => x.Title == Constants.Shows.OldestPremiered).Should().Be(1);

            var card = stat.TopCards.First(x => x.Title == Constants.Shows.OldestPremiered);
            card.Should().NotBeNull();
            card.Title.Should().Be(Constants.Shows.OldestPremiered);
            card.Unit.Should().Be("COMMON.DATE");
            card.Values[0].Value.Should().Be(_showTwo.PremiereDate?.ToString("O"));
            card.Values[0].Label.Should().Be(_showTwo.Name);
            card.UnitNeedsTranslation.Should().Be(true);
            card.ValueType.Should().Be(ValueTypeEnum.Date);
        }

        [Fact]
        public void GetShowWithMostEpisodes()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.TopCards.Count(x => x.Title == Constants.Shows.MostEpisodes).Should().Be(1);

            var card = stat.TopCards.First(x => x.Title == Constants.Shows.MostEpisodes);
            card.Should().NotBeNull();
            card.Title.Should().Be(Constants.Shows.MostEpisodes);
            card.Unit.Should().Be("#");
            card.Values[0].Value.Should().Be(_showTwo.Episodes.Count.ToString());
            card.Values[0].Label.Should().Be(_showTwo.Name);
            card.UnitNeedsTranslation.Should().Be(false);
            card.ValueType.Should().Be(ValueTypeEnum.None);
        }

        [Fact]
        public void GetLatestAddedShow()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.TopCards.Count(x => x.Title == Constants.Shows.LatestAdded).Should().Be(1);

            var card = stat.TopCards.First(x => x.Title == Constants.Shows.LatestAdded);
            card.Should().NotBeNull();
            card.Title.Should().Be(Constants.Shows.LatestAdded);
            card.Unit.Should().Be("COMMON.DATE");
            card.Values[0].Value.Should().Be(_showThree.DateCreated?.ToString("O"));
            card.Values[0].Label.Should().Be(_showThree.Name);
            card.UnitNeedsTranslation.Should().Be(true);
            card.ValueType.Should().Be(ValueTypeEnum.Date);
        }

        [Fact]
        public void GetNewestPremieredShow()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.TopCards.Count(x => x.Title == Constants.Shows.NewestPremiered).Should().Be(1);

            var card = stat.TopCards.First(x => x.Title == Constants.Shows.NewestPremiered);
            card.Should().NotBeNull();
            card.Title.Should().Be(Constants.Shows.NewestPremiered);
            card.Unit.Should().Be("COMMON.DATE");
            card.Values[0].Value.Should().Be(_showThree.PremiereDate?.ToString("O"));
            card.Values[0].Label.Should().Be(_showThree.Name);
            card.UnitNeedsTranslation.Should().Be(true);
            card.ValueType.Should().Be(ValueTypeEnum.Date);
        }

        [Fact]
        public void GetTotalDiskSize()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Cards.Count(x => x.Title == Constants.Common.TotalDiskSize).Should().Be(1);

            var card = stat.Cards.First(x => x.Title == Constants.Common.TotalDiskSize);
            card.Should().NotBeNull();
            card.Title.Should().Be(Constants.Common.TotalDiskSize);
            card.Value.Should().Be("909");
        }

        #endregion

        #region Charts

        [Fact]
        public void GetGenreChart()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.BarCharts.Should().NotBeNull();
            stat.BarCharts.Count.Should().Be(4);
            stat.BarCharts.Any(x => x.Title == Constants.CountPerGenre).Should().BeTrue();

            var graph = stat.BarCharts.Single(x => x.Title == Constants.CountPerGenre);
            graph.Should().NotBeNull();
            graph.SeriesCount.Should().Be(1);
            graph.DataSets.Should().Be("[{\"Label\":\"Action\",\"Val0\":3},{\"Label\":\"Comedy\",\"Val0\":2},{\"Label\":\"Drama\",\"Val0\":1},{\"Label\":\"War\",\"Val0\":1}]");
        }

        [Fact]
        public void GetRatingChart()
        {
            var showFour = new ShowBuilder(Guid.NewGuid().ToString(), _collections.First().Id).AddCommunityRating(9.3f).Build();
            var subject = CreateShowService(_showOne, _showTwo, _showThree, showFour);

            var stat = subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Should().NotBeNull();
            stat.BarCharts.Count.Should().Be(4);
            stat.BarCharts.Any(x => x.Title == Constants.CountPerCommunityRating).Should().BeTrue();

            var graph = stat.BarCharts.Single(x => x.Title == Constants.CountPerCommunityRating);
            graph.Should().NotBeNull();
            graph.SeriesCount.Should().Be(1);
            var dataSet = "{\"Label\":\"0\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 0.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"1\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 1.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"2\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 2.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"3\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 3.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"4\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 4.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"5\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 5.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"6\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 6.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"7\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 7.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"8\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 8.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":2},";
            dataSet += "{\"Label\":\"9\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 9.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":1},";
            dataSet += "{\"Label\":\"UNKNOWN\",\"Val0\":1}";
            graph.DataSets.Should().Be("[" + dataSet + "]");
        }

        [Fact]
        public void GetPremiereYearChart()
        {
            var showFour = new ShowBuilder(Guid.NewGuid().ToString(), _collections.First().Id).AddPremiereDate(new DateTime(2002, 1, 10)).Build();
            var subject = CreateShowService(_showOne, _showTwo, _showThree, showFour);

            var stat = subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Should().NotBeNull();
            stat.BarCharts.Count.Should().Be(4);
            stat.BarCharts.Any(x => x.Title == Constants.CountPerPremiereYear).Should().BeTrue();

            var graph = stat.BarCharts.Single(x => x.Title == Constants.CountPerPremiereYear);
            graph.Should().NotBeNull();
            graph.SeriesCount.Should().Be(1);
            graph.DataSets.Should().Be("[{\"Label\":\"1990 - 1994\",\"Val0\":1},{\"Label\":\"1995 - 1999\",\"Val0\":0},{\"Label\":\"2000 - 2004\",\"Val0\":2},{\"Label\":\"2005 - 2009\",\"Val0\":0},{\"Label\":\"2010 - 2014\",\"Val0\":0},{\"Label\":\"2015 - 2019\",\"Val0\":1}]");
        }

        [Fact]
        public void GetCollectedRateChart()
        {
            var showFour = new ShowBuilder(Guid.NewGuid().ToString(), _collections.First().Id).ClearEpisodes().Build();
            var subject = CreateShowService(_showOne, _showTwo, _showThree, showFour);
            var stat = subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Should().NotBeNull();
            stat.BarCharts.Count.Should().Be(4);
            stat.BarCharts.Any(x => x.Title == Constants.CountPerCollectedPercentage).Should().BeTrue();

            var graph = stat.BarCharts.Single(x => x.Title == Constants.CountPerCollectedPercentage);
            graph.Should().NotBeNull();
            graph.SeriesCount.Should().Be(1);
            graph.DataSets.Should().Be("[{\"Label\":\"0% - 4%\",\"Val0\":1},{\"Label\":\"5% - 9%\",\"Val0\":0},{\"Label\":\"10% - 14%\",\"Val0\":0},{\"Label\":\"15% - 19%\",\"Val0\":0},{\"Label\":\"20% - 24%\",\"Val0\":0},{\"Label\":\"25% - 29%\",\"Val0\":1},{\"Label\":\"30% - 34%\",\"Val0\":0},{\"Label\":\"35% - 39%\",\"Val0\":0},{\"Label\":\"40% - 44%\",\"Val0\":0},{\"Label\":\"45% - 49%\",\"Val0\":0},{\"Label\":\"50% - 54%\",\"Val0\":0},{\"Label\":\"55% - 59%\",\"Val0\":0},{\"Label\":\"60% - 64%\",\"Val0\":1},{\"Label\":\"65% - 69%\",\"Val0\":0},{\"Label\":\"70% - 74%\",\"Val0\":0},{\"Label\":\"75% - 79%\",\"Val0\":0},{\"Label\":\"80% - 84%\",\"Val0\":0},{\"Label\":\"85% - 89%\",\"Val0\":0},{\"Label\":\"90% - 94%\",\"Val0\":0},{\"Label\":\"95% - 99%\",\"Val0\":0}]");
        }

        [Fact]
        public void GetOfficialRatingChart()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Should().NotBeNull();
            stat.PieCharts.Count.Should().Be(2);
            stat.PieCharts.Any(x => x.Title == Constants.CountPerOfficialRating).Should().BeTrue();

            var graph = stat.PieCharts.Single(x => x.Title == Constants.CountPerOfficialRating);
            graph.Should().NotBeNull();
            graph.SeriesCount.Should().Be(1);
            graph.DataSets.Should().Be("[{\"Label\":\"R\",\"Val0\":2},{\"Label\":\"TV-16\",\"Val0\":1}]");
        }

        [Fact]
        public void GetShowStateChart()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Should().NotBeNull();
            stat.PieCharts.Count.Should().Be(2);
            stat.PieCharts.Any(x => x.Title == Constants.Shows.ShowStatusChart).Should().BeTrue();

            var graph = stat.PieCharts.Single(x => x.Title == Constants.Shows.ShowStatusChart);
            graph.Should().NotBeNull();
            graph.SeriesCount.Should().Be(1);
            graph.DataSets.Should().Be("[{\"Label\":\"Continuing\",\"Val0\":2},{\"Label\":\"Ended\",\"Val0\":1}]");
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
        //    var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

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
}
