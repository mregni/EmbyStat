using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
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

        public ShowServiceTests()
        {
            _collections = new List<Library>
            {
                new Library{ Id = string.Empty, Name = "collection1", PrimaryImage = "image1", Type = LibraryType.TvShow},
                new Library{ Id = string.Empty, Name = "collection2", PrimaryImage = "image2", Type = LibraryType.TvShow}
            };

            _showOne = new ShowBuilder(1, _collections.First().Id)
                .AddName("Chuck")
                .AddCreateDate(new DateTime(1990, 4, 2))
                .AddGenre("Comedy", "Action")
                .Build();
            _showTwo = new ShowBuilder(2, _collections.First().Id)
                .AddName("The 100")
                .AddMissingEpisodes(10, 1)
                .AddCommunityRating(8.3f)
                .AddPremiereDate(new DateTime(1992, 4, 1))
                .AddEpisode(new EpisodeBuilder(3, 2, "1").Build())
                .AddEpisode(new EpisodeBuilder(4, 2, "1").Build())
                .AddGenre("Drama", "Comedy", "Action")
                .SetContinuing()
                .AddOfficialRating("TV-16")
                .AddActor(_showOne.People.First().Id)
                .Build();
            _showThree = new ShowBuilder(3, _collections.First().Id)
                .AddName("Dexter")
                .AddMissingEpisodes(2, 1)
                .AddCommunityRating(8.4f)
                .AddPremiereDate(new DateTime(2018, 4, 10))
                .AddEpisode(new EpisodeBuilder(3, 3, "1").Build())
                .AddCreateDate(new DateTime(2003, 4, 2))
                .AddGenre("War", "Action")
                .SetContinuing()
                .Build();

            _subject = CreateShowService(_showOne, _showTwo, _showThree);
        }

        private ShowService CreateShowService(params Show[] shows)
        {
            var showRepositoryMock = new Mock<IShowRepository>();
            showRepositoryMock.Setup(x => x.GetAllShows(It.IsAny<IReadOnlyList<string>>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(shows);
            foreach (var show in shows)
            {
                showRepositoryMock.Setup(x => x.GetAllEpisodesForShow(show.Id)).Returns(show.Episodes);
            }

            var collectionRepositoryMock = new Mock<ILibraryRepository>();
            collectionRepositoryMock.Setup(x => x.GetLibrariesByTypes(It.IsAny<IEnumerable<LibraryType>>())).Returns(_collections);

            var personServiceMock = new Mock<IPersonService>();
            foreach (var person in shows.SelectMany(x => x.People))
            {
                personServiceMock.Setup(x => x.GetPersonByNameAsync(person.Name)).Returns(
                    Task.FromResult(new Person
                    {
                        Id = person.Id,
                        Name = person.Name,
                        BirthDate = new DateTime(2000, 1, 1),
                        Primary = "primary.jpg",
                        MovieCount = 0,
                        ShowCount = 0
                    }));
            }

            var settingsServiceMock = new Mock<ISettingsService>();
            settingsServiceMock.Setup(x => x.GetUserSettings())
                .Returns(new UserSettings { ShowLibraryTypes = new List<LibraryType> { LibraryType.TvShow } });
            var statisticsRepositoryMock = new Mock<IStatisticsRepository>();
            var jobRepositoryMock = new Mock<IJobRepository>();
            return new ShowService(jobRepositoryMock.Object, showRepositoryMock.Object, collectionRepositoryMock.Object, personServiceMock.Object, statisticsRepositoryMock.Object, settingsServiceMock.Object);
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
        public async void GetTotalShowCount()
        {
            var stat = await _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.ShowCount.Should().NotBeNull();
            stat.General.ShowCount.Title.Should().Be(Constants.Shows.TotalShows);
            stat.General.ShowCount.Value.Should().Be(3);
        }

        [Fact]
        public async void GetTotalEpisodeCount()
        {
            var stat = await _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.EpisodeCount.Should().NotBeNull();
            stat.General.EpisodeCount.Title.Should().Be(Constants.Shows.TotalEpisodes);
            stat.General.EpisodeCount.Value.Should().Be(9);
        }

        [Fact]
        public async void GetTotalMissingEpisodeCount()
        {
            var stat = await _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.MissingEpisodeCount.Should().NotBeNull();
            stat.General.MissingEpisodeCount.Title.Should().Be(Constants.Shows.TotalMissingEpisodes);
            stat.General.MissingEpisodeCount.Value.Should().Be(12);
        }

        [Fact]
        public async void GetCalculatePlayableTime()
        {
            var stat = await _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.TotalPlayableTime.Should().NotBeNull();
            stat.General.TotalPlayableTime.Title.Should().Be(Constants.Shows.TotalPlayLength);
            stat.General.TotalPlayableTime.Days.Should().Be(67);
            stat.General.TotalPlayableTime.Hours.Should().Be(8);
            stat.General.TotalPlayableTime.Minutes.Should().Be(41);
            stat.General.TotalPlayableTime.Value.Should().BeNull();
        }

        [Fact]
        public async void GetHighestRatedShow()
        {
            var stat = await _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.HighestRatedShow.Should().NotBeNull();
            stat.General.HighestRatedShow.CommunityRating.Should().Be($"{_showThree.CommunityRating:0.0}");
            stat.General.HighestRatedShow.MediaId.Should().Be(_showThree.Id);
            stat.General.HighestRatedShow.Name.Should().Be(_showThree.Name);
            stat.General.HighestRatedShow.OfficialRating.Should().Be(_showThree.OfficialRating);
            stat.General.HighestRatedShow.Year.Should().Be(_showThree.PremiereDate?.Year ?? 0);
            stat.General.HighestRatedShow.Tag.Should().Be(_showThree.Primary);
            stat.General.HighestRatedShow.Title.Should().Be(Constants.Shows.HighestRatedShow);
        }

        [Fact]
        public async void GetLowestRatedShow()
        {
            var stat = await _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.LowestRatedShow.Should().NotBeNull();
            stat.General.LowestRatedShow.CommunityRating.Should().Be($"{_showTwo.CommunityRating:0.0}");
            stat.General.LowestRatedShow.MediaId.Should().Be(_showTwo.Id);
            stat.General.LowestRatedShow.Name.Should().Be(_showTwo.Name);
            stat.General.LowestRatedShow.OfficialRating.Should().Be(_showTwo.OfficialRating);
            stat.General.LowestRatedShow.Year.Should().Be(_showTwo.PremiereDate?.Year ?? 0);
            stat.General.LowestRatedShow.Tag.Should().Be(_showTwo.Primary);
            stat.General.LowestRatedShow.Title.Should().Be(Constants.Shows.LowestRatedShow);
        }

        [Fact]
        public async void GetOldestPremieredShow()
        {
            var stat = await _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.OldestPremieredShow.Should().NotBeNull();
            stat.General.OldestPremieredShow.CommunityRating.Should().Be($"{_showTwo.CommunityRating:0.0}");
            stat.General.OldestPremieredShow.MediaId.Should().Be(_showTwo.Id);
            stat.General.OldestPremieredShow.Name.Should().Be(_showTwo.Name);
            stat.General.OldestPremieredShow.OfficialRating.Should().Be(_showTwo.OfficialRating);
            stat.General.OldestPremieredShow.Year.Should().Be(_showTwo.PremiereDate?.Year ?? 0);
            stat.General.OldestPremieredShow.Tag.Should().Be(_showTwo.Primary);
            stat.General.OldestPremieredShow.Title.Should().Be(Constants.Shows.OldestPremiered);
        }

        [Fact]
        public async void GetShowWithMostEpisodes()
        {
            var stat = await _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.ShowWithMostEpisodes.Should().NotBeNull();
            stat.General.ShowWithMostEpisodes.CommunityRating.Should().Be($"{_showTwo.CommunityRating:0.0}");
            stat.General.ShowWithMostEpisodes.MediaId.Should().Be(_showTwo.Id);
            stat.General.ShowWithMostEpisodes.Name.Should().Be(_showTwo.Name);
            stat.General.ShowWithMostEpisodes.OfficialRating.Should().Be(_showTwo.OfficialRating);
            stat.General.ShowWithMostEpisodes.Year.Should().Be(_showTwo.PremiereDate?.Year ?? 0);
            stat.General.ShowWithMostEpisodes.Tag.Should().Be(_showTwo.Primary);
            stat.General.ShowWithMostEpisodes.Title.Should().Be(Constants.Shows.MostEpisodes);
        }

        [Fact]
        public async void GetLatestAddedShow()
        {
            var stat = await _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.LatestAddedShow.Should().NotBeNull();
            stat.General.LatestAddedShow.CommunityRating.Should().Be($"{_showThree.CommunityRating:0.0}");
            stat.General.LatestAddedShow.MediaId.Should().Be(_showThree.Id);
            stat.General.LatestAddedShow.Name.Should().Be(_showThree.Name);
            stat.General.LatestAddedShow.OfficialRating.Should().Be(_showThree.OfficialRating);
            stat.General.LatestAddedShow.Year.Should().Be(_showThree.PremiereDate?.Year ?? 0);
            stat.General.LatestAddedShow.Tag.Should().Be(_showThree.Primary);
            stat.General.LatestAddedShow.Title.Should().Be(Constants.Shows.LatestAdded);
        }

        [Fact]
        public async void GetNewestPremieredShow()
        {
            var stat = await _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.NewestPremieredShow.Should().NotBeNull();
            stat.General.NewestPremieredShow.CommunityRating.Should().Be($"{_showThree.CommunityRating:0.0}");
            stat.General.NewestPremieredShow.MediaId.Should().Be(_showThree.Id);
            stat.General.NewestPremieredShow.Name.Should().Be(_showThree.Name);
            stat.General.NewestPremieredShow.OfficialRating.Should().Be(_showThree.OfficialRating);
            stat.General.NewestPremieredShow.Year.Should().Be(_showThree.PremiereDate?.Year ?? 0);
            stat.General.NewestPremieredShow.Tag.Should().Be(_showThree.Primary);
            stat.General.NewestPremieredShow.Title.Should().Be(Constants.Shows.NewestPremiered);
        }

        [Fact]
        public async void GetTotalDiskSize()
        {
            var stat = await _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.TotalDiskSize.Should().NotBeNull();
            stat.General.TotalDiskSize.Title.Should().Be(Constants.Common.TotalDiskSize);
            stat.General.TotalDiskSize.Value.Should().Be(909);
        }

        #endregion

        #region Charts

        [Fact]
        public async void GetGenreChart()
        {
            var stat = await _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Charts.Should().NotBeNull();
            stat.Charts.BarCharts.Count.Should().Be(4);
            stat.Charts.BarCharts.Any(x => x.Title == Constants.CountPerGenre).Should().BeTrue();

            var bar = stat.Charts.BarCharts.Single(x => x.Title == Constants.CountPerGenre);
            bar.Labels.Count().Should().Be(4);

            var labels = bar.Labels.ToArray();
            labels[0].Should().Be("Action");
            labels[1].Should().Be("Comedy");
            labels[2].Should().Be("Drama");
            labels[3].Should().Be("War");

            bar.DataSets.Count.Should().Be(1);
            var dataSet = bar.DataSets[0].ToArray();
            dataSet[0].Should().Be(3);
            dataSet[1].Should().Be(2);
            dataSet[2].Should().Be(1);
            dataSet[3].Should().Be(1);
        }

        [Fact]
        public async void GetRatingChart()
        {
            var showFour = new ShowBuilder(4, _collections.First().Id).AddCommunityRating(9.3f).Build();
            var subject = CreateShowService(_showOne, _showTwo, _showThree, showFour);

            var stat = await subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Charts.Should().NotBeNull();
            stat.Charts.BarCharts.Count.Should().Be(4);
            stat.Charts.BarCharts.Any(x => x.Title == Constants.CountPerCommunityRating).Should().BeTrue();

            var bar = stat.Charts.BarCharts.Single(x => x.Title == Constants.CountPerCommunityRating);
            bar.Labels.Count().Should().Be(21);
            for (var i = 0; i < 20; i++)
            {
                bar.Labels.ToArray()[i].Should().Be((i * (float)0.5).ToString());
            }

            bar.Labels.Last().Should().Be(Constants.Unknown);

            var dataSet = bar.DataSets.Single().ToList();
            dataSet.Count.Should().Be(21);
            for (var i = 0; i < 16; i++)
            {
                dataSet[i].Should().Be(0);
            }
            dataSet[17].Should().Be(2);
            dataSet[18].Should().Be(0);
            dataSet[19].Should().Be(1);
            dataSet[20].Should().Be(1);
        }

        [Fact]
        public async void GetPremiereYearChart()
        {
            var showFour = new ShowBuilder(4, _collections.First().Id).AddPremiereDate(new DateTime(2002, 1, 10)).Build();
            var subject = CreateShowService(_showOne, _showTwo, _showThree, showFour);

            var stat = await subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Charts.Should().NotBeNull();
            stat.Charts.BarCharts.Count.Should().Be(4);
            stat.Charts.BarCharts.Any(x => x.Title == Constants.CountPerPremiereYear).Should().BeTrue();

            var bar = stat.Charts.BarCharts.Single(x => x.Title == Constants.CountPerPremiereYear);
            bar.Labels.Count().Should().Be(6);
            var labels = bar.Labels.ToArray();

            labels[0].Should().Be("1990 - 1994");
            labels[1].Should().Be("1995 - 1999");
            labels[2].Should().Be("2000 - 2004");
            labels[3].Should().Be("2005 - 2009");
            labels[4].Should().Be("2010 - 2014");
            labels[5].Should().Be("2015 - 2019");

            var dataSet = bar.DataSets.Single().ToArray();
            dataSet.Length.Should().Be(6);

            dataSet[0].Should().Be(1);
            dataSet[1].Should().Be(0);
            dataSet[2].Should().Be(2);
            dataSet[3].Should().Be(0);
            dataSet[4].Should().Be(0);
            dataSet[5].Should().Be(1);
        }

        [Fact]
        public async void GetCollectedRateChart()
        {
            var showFour = new ShowBuilder(4, _collections.First().Id).ClearEpisodes().Build();
            var subject = CreateShowService(_showOne, _showTwo, _showThree, showFour);
            var stat = await subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Charts.Should().NotBeNull();
            stat.Charts.BarCharts.Count.Should().Be(4);
            stat.Charts.BarCharts.Any(x => x.Title == Constants.CountPerCollectedRate).Should().BeTrue();

            var bar = stat.Charts.BarCharts.Single(x => x.Title == Constants.CountPerCollectedRate);
            bar.Labels.Count().Should().Be(21);
            var labels = bar.Labels.ToArray();

            labels[0].Should().Be("0% - 4%");
            labels[1].Should().Be("5% - 9%");
            labels[2].Should().Be("10% - 14%");
            labels[3].Should().Be("15% - 19%");
            labels[4].Should().Be("20% - 24%");
            labels[5].Should().Be("25% - 29%");
            labels[6].Should().Be("30% - 34%");
            labels[7].Should().Be("35% - 39%");
            labels[8].Should().Be("40% - 44%");
            labels[9].Should().Be("45% - 49%");
            labels[10].Should().Be("50% - 54%");
            labels[11].Should().Be("55% - 59%");
            labels[12].Should().Be("60% - 64%");
            labels[13].Should().Be("65% - 69%");
            labels[14].Should().Be("70% - 74%");
            labels[15].Should().Be("75% - 79%");
            labels[16].Should().Be("80% - 84%");
            labels[17].Should().Be("85% - 89%");
            labels[18].Should().Be("90% - 94%");
            labels[19].Should().Be("95% - 99%");
            labels[20].Should().Be("100%");

            bar.DataSets[0].Count().Should().Be(21);
            var dataSet = bar.DataSets[0].ToArray();
            dataSet[0].Should().Be(1);
            dataSet[1].Should().Be(0);
            dataSet[2].Should().Be(0);
            dataSet[3].Should().Be(0);
            dataSet[4].Should().Be(0);
            dataSet[5].Should().Be(1);
            dataSet[6].Should().Be(0);
            dataSet[7].Should().Be(0);
            dataSet[8].Should().Be(0);
            dataSet[9].Should().Be(0);
            dataSet[10].Should().Be(0);
            dataSet[11].Should().Be(0);
            dataSet[12].Should().Be(1);
            dataSet[13].Should().Be(0);
            dataSet[14].Should().Be(0);
            dataSet[15].Should().Be(0);
            dataSet[16].Should().Be(0);
            dataSet[17].Should().Be(0);
            dataSet[18].Should().Be(0);
            dataSet[19].Should().Be(0);
            dataSet[20].Should().Be(1);
        }

        [Fact]
        public async void GetOfficialRatingChart()
        {
            var stat = await _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Charts.Should().NotBeNull();
            stat.Charts.PieCharts.Count.Should().Be(2);
            stat.Charts.PieCharts.Any(x => x.Title == Constants.CountPerOfficialRating).Should().BeTrue();

            var pie = stat.Charts.PieCharts.Single(x => x.Title == Constants.CountPerOfficialRating);
            pie.Labels.Count().Should().Be(2);

            var labels = pie.Labels.ToArray();
            labels[0].Should().Be("R");
            labels[1].Should().Be("TV-16");

            pie.DataSets[0].Count().Should().Be(2);
            var dataSet = pie.DataSets[0].ToArray();

            dataSet[0].Should().Be(2);
            dataSet[1].Should().Be(1);

        }

        [Fact]
        public async void GetShowStateChart()
        {
            var stat = await _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Charts.Should().NotBeNull();
            stat.Charts.PieCharts.Count.Should().Be(2);
            stat.Charts.PieCharts.Any(x => x.Title == Constants.Shows.ShowStatusChart).Should().BeTrue();

            var pie = stat.Charts.PieCharts.Single(x => x.Title == Constants.Shows.ShowStatusChart);
            pie.Labels.Count().Should().Be(2);

            var labels = pie.Labels.ToArray();
            labels[0].Should().Be("Continuing");
            labels[1].Should().Be("Ended");

            pie.DataSets[0].Count().Should().Be(2);
            var dataSet = pie.DataSets[0].ToArray();

            dataSet[0].Should().Be(2);
            dataSet[1].Should().Be(1);
        }

        #endregion

        #region People

        [Fact]
        public async void GetMostFeaturedActorsPerGenreAsync()
        {
            var stat = await _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.People.Should().NotBeNull();
            stat.People.MostFeaturedActorsPerGenre.Should().NotBeNull();
            stat.People.MostFeaturedActorsPerGenre.Count.Should().Be(4);
            stat.People.MostFeaturedActorsPerGenre[0].Title.Should().Be("Action");
            stat.People.MostFeaturedActorsPerGenre[1].Title.Should().Be("Comedy");
            stat.People.MostFeaturedActorsPerGenre[2].Title.Should().Be("Drama");
            stat.People.MostFeaturedActorsPerGenre[3].Title.Should().Be("War");
        }

        #endregion
    }
}
