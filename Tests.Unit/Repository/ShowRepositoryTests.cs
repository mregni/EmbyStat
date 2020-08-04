using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Repositories;
using FluentAssertions;
using Tests.Unit.Builders;
using Xunit;

namespace Tests.Unit.Repository
{
    public class ShowRepositoryTests : BaseRepositoryTester
    {
        private ShowRepository _showRepository;
        private DbContext _context;

        public ShowRepositoryTests() : base("test-data-show-repo.db")
        {

        }

        protected override void SetupRepository()
        {
            _context = CreateDbContext();
            _showRepository = new ShowRepository(_context);
        }

        [Fact]
        public void GetShowById_Should_Return_Correct_Show()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").AddName("Wallander").Build();
                var showTwo = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<Show>();
                    collection.InsertBulk(new[] { showOne, showTwo });
                }

                var show = _showRepository.GetShowById(showOne.Id);
                show.Should().NotBeNull();
                show.Id.Should().Be(showOne.Id);
                show.Name.Should().Be(showOne.Name);
            });
        }

        [Fact]
        public void AddSeason_Should_Add_The_Season()
        {
            RunTest(() =>
            {
                var seasonOne = new SeasonBuilder(Guid.NewGuid().ToString(), "1").Build();
                _showRepository.AddSeason(seasonOne);

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<Season>();
                    var season = collection.FindById(seasonOne.Id);
                    season.Should().NotBeNull();
                    season.Id.Should().Be(seasonOne.Id);
                }
            });
        }

        [Fact]
        public void AddEpisode_Should_Add_The_Episode()
        {
            RunTest(() =>
            {
                var episodeOne = new EpisodeBuilder(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "1").Build();
                _showRepository.AddEpisode(episodeOne);

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<Episode>();
                    var episode = collection.FindById(episodeOne.Id);
                    episode.Should().NotBeNull();
                    episode.Id.Should().Be(episodeOne.Id);
                }
            });
        }

        [Fact]
        public void InsertShow_Should_Succeed()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();

                _showRepository.InsertShow(showOne);

                var showList = _showRepository.GetAllShows(new string[0], false, false);
                showList.Should().NotBeNull();
                showList.Should().NotContainNulls();
                showList.Count.Should().Be(1);

                showList.First().Id.Should().Be(showOne.Id);

                var count = _showRepository.GetMediaCount(new string[0]);
                count.Should().Be(1);
            });
        }

        [Fact]
        public void GetMediaCount_Should_Return_Correct_Count()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                var showTwo = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();

                _showRepository.InsertShow(showOne);
                _showRepository.InsertShow(showTwo);

                var count = _showRepository.GetMediaCount(new string[0]);
                count.Should().Be(2);
            });
        }

        [Fact]
        public void GetMediaCount_With_Library_Filter_Should_Return_Correct_Count()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                var showTwo = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                var showThree = new ShowBuilder(Guid.NewGuid().ToString(), "2").Build();

                _showRepository.InsertShow(showOne);
                _showRepository.InsertShow(showTwo);
                _showRepository.InsertShow(showThree);

                var count = _showRepository.GetMediaCount(new[] { "2" });
                count.Should().Be(1);
            });
        }

        [Fact]
        public void RemoveShowsThatAreNotUpdated_Should_Remove_All_Shows_That_Are_Not_Updated()
        {
            RunTest(() =>
            {
                var now = DateTime.Now;
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").AddUpdateState(now.AddDays(-1)).Build();
                var showTwo = new ShowBuilder(Guid.NewGuid().ToString(), "1").AddUpdateState(now.AddDays(1)).Build();

                _showRepository.InsertShow(showOne);
                _showRepository.InsertShow(showTwo);

                var count = _showRepository.GetMediaCount(new string[0]);
                count.Should().Be(2);

                _showRepository.RemoveShowsThatAreNotUpdated(now);

                var shows = _showRepository.GetAllShows(new string[0], true, true);
                shows.Should().NotContainNulls();
                shows.Count.Should().Be(1);

                var episodes = _showRepository.GetAllEpisodesForShow(showOne.Id);
                episodes.Should().NotContainNulls();
                episodes.Should().BeEmpty();

                episodes = _showRepository.GetAllEpisodesForShow(showTwo.Id);
                episodes.Should().NotContainNulls();
                episodes.Should().NotBeEmpty();
            });
        }

        [Fact]
        public void GetAllShows_Should_Return_All_Shows_Without_Seasons_Or_Episodes()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                _showRepository.InsertShow(showOne);

                var shows = _showRepository.GetAllShows(new string[0], false, false);
                shows.Should().NotContainNulls();
                shows.Count.Should().Be(1);

                shows[0].Episodes.Count.Should().Be(2);
                shows[0].Episodes[0].Id.Should().Be(showOne.Episodes[0].Id);
                shows[0].Episodes[0].Name.Should().BeNull();
                shows[0].Episodes[1].Id.Should().Be(showOne.Episodes[1].Id);
                shows[0].Episodes[1].Name.Should().BeNull();

                shows[0].Seasons.Count.Should().Be(1);
                shows[0].Seasons[0].Id.Should().Be(showOne.Seasons[0].Id);
                shows[0].Seasons[0].Name.Should().BeNull();
            });
        }

        [Fact]
        public void GetAllShows_With_Library_Filter_Should_Return_All_Shows_Without_Seasons_Or_Episodes()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                var showTwo = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                var showThree = new ShowBuilder(Guid.NewGuid().ToString(), "2").Build();

                _showRepository.InsertShow(showOne);
                _showRepository.InsertShow(showTwo);
                _showRepository.InsertShow(showThree);

                var shows = _showRepository.GetAllShows(new[] { "2" }, false, false);
                shows.Should().NotContainNulls();
                shows.Count.Should().Be(1);

                shows[0].Episodes.Count.Should().Be(2);
                shows[0].Episodes[0].Id.Should().Be(showThree.Episodes[0].Id);
                shows[0].Episodes[0].Name.Should().BeNull();
                shows[0].Episodes[1].Id.Should().Be(showThree.Episodes[1].Id);
                shows[0].Episodes[1].Name.Should().BeNull();

                shows[0].Seasons.Count.Should().Be(1);
                shows[0].Seasons[0].Id.Should().Be(showThree.Seasons[0].Id);
                shows[0].Seasons[0].Name.Should().BeNull();
            });
        }

        [Fact]
        public void GetAllShows_Should_Return_All_Shows_With_Episodes_And_Without_Seasons()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                _showRepository.InsertShow(showOne);

                var shows = _showRepository.GetAllShows(new string[0], false, true);
                shows.Should().NotContainNulls();
                shows.Count.Should().Be(1);

                shows[0].Episodes.Count.Should().Be(2);
                shows[0].Episodes[0].Id.Should().Be(showOne.Episodes[0].Id);
                shows[0].Episodes[0].Name.Should().Be(showOne.Episodes[0].Name);
                shows[0].Episodes[1].Id.Should().Be(showOne.Episodes[1].Id);
                shows[0].Episodes[1].Name.Should().Be(showOne.Episodes[1].Name);

                shows[0].Seasons.Count.Should().Be(1);
                shows[0].Seasons[0].Id.Should().Be(showOne.Seasons[0].Id);
                shows[0].Seasons[0].Name.Should().BeNull();
            });
        }

        [Fact]
        public void GetAllShows_Should_Return_All_Shows_With_Seasons_And_Without_Episodes()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                _showRepository.InsertShow(showOne);

                var shows = _showRepository.GetAllShows(new string[0], true, false);
                shows.Should().NotContainNulls();
                shows.Count.Should().Be(1);

                shows[0].Episodes.Count.Should().Be(2);
                shows[0].Episodes[0].Id.Should().Be(showOne.Episodes[0].Id);
                shows[0].Episodes[0].Name.Should().BeNull();
                shows[0].Episodes[1].Id.Should().Be(showOne.Episodes[1].Id);
                shows[0].Episodes[1].Name.Should().BeNull();

                shows[0].Seasons.Count.Should().Be(1);
                shows[0].Seasons[0].Id.Should().Be(showOne.Seasons[0].Id);
                shows[0].Seasons[0].Name.Should().Be(showOne.Seasons[0].Name);
            });
        }

        [Fact]
        public void GetAllShows_Should_Return_All_Shows_With_Seasons_And_Episodes()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                _showRepository.InsertShow(showOne);

                var shows = _showRepository.GetAllShows(new string[0], true, true);
                shows.Should().NotContainNulls();
                shows.Count.Should().Be(1);

                shows[0].Episodes.Count.Should().Be(2);
                shows[0].Episodes[0].Id.Should().Be(showOne.Episodes[0].Id);
                shows[0].Episodes[0].Name.Should().Be(showOne.Episodes[0].Name);
                shows[0].Episodes[1].Id.Should().Be(showOne.Episodes[1].Id);
                shows[0].Episodes[1].Name.Should().Be(showOne.Episodes[1].Name);

                shows[0].Seasons.Count.Should().Be(1);
                shows[0].Seasons[0].Id.Should().Be(showOne.Seasons[0].Id);
                shows[0].Seasons[0].Name.Should().Be(showOne.Seasons[0].Name);
            });
        }

        [Fact]
        public void Should_Return_Correct_Season()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                _showRepository.InsertShow(showOne);

                var season = _showRepository.GetSeasonById(showOne.Seasons.First().Id);
                season.Should().NotBeNull();
                season.Id.Should().Be(showOne.Seasons.First().Id);
                season.ParentId.Should().Be(showOne.Id);
            });
        }

        [Fact]
        public void Should_Return_Correct_Episode()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                _showRepository.InsertShow(showOne);

                var episode = _showRepository.GetEpisodeById(showOne.Id, showOne.Episodes.First().Id);
                episode.Should().NotBeNull();
                episode.Id.Should().Be(showOne.Episodes.First().Id);
                episode.ParentId.Should().Be("1");
            });
        }

        [Fact]
        public void Any_Should_Return_True_If_Any_Shows_Are_In_Database()
        {
            RunTest(() =>
            {
                var movieOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                _showRepository.InsertShow(movieOne);

                var result = _showRepository.Any();
                result.Should().BeTrue();
            });
        }

        [Fact]
        public void Any_Should_Return_False_If_No_Shows_Are_In_Database()
        {
            RunTest(() =>
            {
                var result = _showRepository.Any();
                result.Should().BeFalse();
            });
        }

        [Fact]
        public void GetMediaCountForPerson_Should_Return_Show_Count_For_Person_Id()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                var showTwo = new ShowBuilder(Guid.NewGuid().ToString(), "1").AddPerson(new ExtraPerson { Id = showOne.People.First().Id, Type = PersonType.Actor, Name = "Test" }).Build();
                var showThree = new ShowBuilder(Guid.NewGuid().ToString(), "2").Build();

                _showRepository.InsertShow(showOne);
                _showRepository.InsertShow(showTwo);
                _showRepository.InsertShow(showThree);

                var count = _showRepository.GetMediaCountForPerson(showOne.People.First().Id);

                count.Should().Be(2);
            });
        }

        [Fact]
        public void GetAllEpisodesForShow_Should_Return_List_Of_Episodes()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                var showTwo = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();

                _showRepository.InsertShow(showOne);
                _showRepository.InsertShow(showTwo);

                var episodes = _showRepository.GetAllEpisodesForShow(showOne.Id);
                episodes.Should().NotContainNulls();
                episodes.Count.Should().Be(2);
                
                episodes.OrderBy(x => x.IndexNumber).ToArray()[0].Id.Should().Be(showOne.Episodes[0].Id);
                episodes.OrderBy(x => x.IndexNumber).ToArray()[1].Id.Should().Be(showOne.Episodes[1].Id);
            });
        }

        [Fact]
        public void GetNewestPremieredMedia_Should_Return_Newest_Premiered_Show()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                var showTwo = new ShowBuilder(Guid.NewGuid().ToString(), "1").AddPremiereDate(new DateTime(1920, 1, 2)).Build();
                var showThree = new ShowBuilder(Guid.NewGuid().ToString(), "2").AddPremiereDate(new DateTime(2019, 1, 2)).Build();
                _showRepository.InsertShow(showOne);
                _showRepository.InsertShow(showTwo);
                _showRepository.InsertShow(showThree);

                var shows = _showRepository
                    .GetNewestPremieredMedia(new string[0], 1)
                    .ToList();

                shows.Should().NotBeNull();
                shows.Count.Should().Be(1);

                shows[0].Id.Should().Be(showThree.Id);
            });
        }

        [Fact]
        public void GetNewestPremieredMedia_With_library_Filter_Should_Return_Newest_Premiered_Show()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                var showTwo = new ShowBuilder(Guid.NewGuid().ToString(), "1").AddPremiereDate(new DateTime(1920, 1, 2)).Build();
                var showThree = new ShowBuilder(Guid.NewGuid().ToString(), "2").AddPremiereDate(new DateTime(2019, 1, 2)).Build();
                _showRepository.InsertShow(showOne);
                _showRepository.InsertShow(showTwo);
                _showRepository.InsertShow(showThree);

                var shows = _showRepository
                    .GetNewestPremieredMedia(new[] { "1" }, 1)
                    .ToList();

                shows.Should().NotBeNull();
                shows.Count.Should().Be(1);

                shows[0].Id.Should().Be(showOne.Id);
            });
        }

        [Fact]
        public void GetOldestPremieredMedia_Should_Return_Oldest_Premiered_Show()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                var showTwo = new ShowBuilder(Guid.NewGuid().ToString(), "1").AddPremiereDate(new DateTime(2019, 1, 2)).Build();
                var showThree = new ShowBuilder(Guid.NewGuid().ToString(), "2").AddPremiereDate(new DateTime(1920, 1, 2)).Build();
                _showRepository.InsertShow(showOne);
                _showRepository.InsertShow(showTwo);
                _showRepository.InsertShow(showThree);

                var shows = _showRepository
                    .GetOldestPremieredMedia(new string[0], 1)
                    .ToList();

                shows.Should().NotBeNull();
                shows.Count.Should().Be(1);

                shows[0].Id.Should().Be(showThree.Id);
            });
        }

        [Fact]
        public void GetOldestPremieredMedia_With_library_Filter_Should_Return_Oldest_Premiered_Show()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                var showTwo = new ShowBuilder(Guid.NewGuid().ToString(), "1").AddPremiereDate(new DateTime(2019, 1, 2)).Build();
                var showThree = new ShowBuilder(Guid.NewGuid().ToString(), "2").AddPremiereDate(new DateTime(1920, 1, 2)).Build();
                _showRepository.InsertShow(showOne);
                _showRepository.InsertShow(showTwo);
                _showRepository.InsertShow(showThree);

                var shows = _showRepository
                    .GetOldestPremieredMedia(new[] {"1"}, 1)
                    .ToList();

                shows.Should().NotBeNull();
                shows.Count.Should().Be(1);

                shows[0].Id.Should().Be(showOne.Id);
            });
        }

        [Fact]
        public void GetHighestRatedMedia_Should_Return_Highest_Rated_Show()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                var showTwo = new ShowBuilder(Guid.NewGuid().ToString(), "1").AddCommunityRating(1).Build();
                var showThree = new ShowBuilder(Guid.NewGuid().ToString(), "2").AddCommunityRating(9).Build();
                _showRepository.InsertShow(showOne);
                _showRepository.InsertShow(showTwo);
                _showRepository.InsertShow(showThree);

                var shows = _showRepository
                    .GetHighestRatedMedia(new string[0], 1)
                    .ToList();

                shows.Should().NotBeNull();
                shows.Count.Should().Be(1);

                shows[0].Id.Should().Be(showThree.Id);
            });
        }

        [Fact]
        public void GetHighestRatedMedia_With_library_Filter_Should_Return_Highest_Rated_Show()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                var showTwo = new ShowBuilder(Guid.NewGuid().ToString(), "1").AddCommunityRating(1).Build();
                var showThree = new ShowBuilder(Guid.NewGuid().ToString(), "2").AddCommunityRating(9).Build();
                _showRepository.InsertShow(showOne);
                _showRepository.InsertShow(showTwo);
                _showRepository.InsertShow(showThree);

                var shows = _showRepository
                    .GetHighestRatedMedia(new[] { "1" }, 1)
                    .ToList();

                shows.Should().NotBeNull();
                shows.Count.Should().Be(1);

                shows[0].Id.Should().Be(showOne.Id);
            });
        }

        [Fact]
        public void GetLowestRatedMedia_Should_Return_Lowest_Rated_Show()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                var showTwo = new ShowBuilder(Guid.NewGuid().ToString(), "1").AddCommunityRating(9).Build();
                var showThree = new ShowBuilder(Guid.NewGuid().ToString(), "2").AddCommunityRating(1).Build();
                _showRepository.InsertShow(showOne);
                _showRepository.InsertShow(showTwo);
                _showRepository.InsertShow(showThree);

                var shows = _showRepository
                    .GetLowestRatedMedia(new string[0], 1)
                    .ToList();

                shows.Should().NotBeNull();
                shows.Count.Should().Be(1);

                shows[0].Id.Should().Be(showThree.Id);
            });
        }

        [Fact]
        public void GetLowestRatedMedia_With_library_Filter_Should_Return_Lowest_Rated_Show()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                var showTwo = new ShowBuilder(Guid.NewGuid().ToString(), "1").AddCommunityRating(9).Build();
                var showThree = new ShowBuilder(Guid.NewGuid().ToString(), "2").AddCommunityRating(1).Build();
                _showRepository.InsertShow(showOne);
                _showRepository.InsertShow(showTwo);
                _showRepository.InsertShow(showThree);

                var shows = _showRepository
                    .GetLowestRatedMedia(new[] {"1"}, 1)
                    .ToList();

                shows.Should().NotBeNull();
                shows.Count.Should().Be(1);

                shows[0].Id.Should().Be(showOne.Id);
            });
        }

        [Fact]
        public void GetLatestAddedMedia_Should_Return_Latest_Added_Show()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                var showTwo = new ShowBuilder(Guid.NewGuid().ToString(), "1").AddCreateDate(new DateTime(2000, 1, 1)).Build();
                var showThree = new ShowBuilder(Guid.NewGuid().ToString(), "2").AddCreateDate(new DateTime(2017, 1, 1)).Build();
                _showRepository.InsertShow(showOne);
                _showRepository.InsertShow(showTwo);
                _showRepository.InsertShow(showThree);

                var shows = _showRepository
                    .GetLatestAddedMedia(new string[0], 1)
                    .ToList();

                shows.Should().NotBeNull();
                shows.Count.Should().Be(1);

                shows[0].Id.Should().Be(showThree.Id);
            });
        }

        [Fact]
        public void GetLatestAddedMedia_With_library_Filter_Should_Return_Latest_Added_Show()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                var showTwo = new ShowBuilder(Guid.NewGuid().ToString(), "1").AddCreateDate(new DateTime(2000, 1, 1)).Build();
                var showThree = new ShowBuilder(Guid.NewGuid().ToString(), "2").AddCreateDate(new DateTime(2017, 1, 1)).Build();
                _showRepository.InsertShow(showOne);
                _showRepository.InsertShow(showTwo);
                _showRepository.InsertShow(showThree);

                var shows = _showRepository
                    .GetLatestAddedMedia(new[] { "1" }, 1)
                    .ToList();

                shows.Should().NotBeNull();
                shows.Count.Should().Be(1);

                shows[0].Id.Should().Be(showOne.Id);
            });
        }

        
        [Fact]
        public void RemoveShows_Should_Remove_All_Shows()
        {
            RunTest(() =>
            {
                var showOne = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
                var showTwo = new ShowBuilder(Guid.NewGuid().ToString(), "1").AddCreateDate(new DateTime(2000, 1, 1)).Build();
                var showThree = new ShowBuilder(Guid.NewGuid().ToString(), "2").AddCreateDate(new DateTime(2017, 1, 1)).Build();
                _showRepository.InsertShow(showOne);
                _showRepository.InsertShow(showTwo);
                _showRepository.InsertShow(showThree);

                _showRepository.RemoveShows();

                var shows = _showRepository.GetAllShows(new List<string>(), false, false);
                shows.Count.Should().Be(0);
            });
        }
    }
}
