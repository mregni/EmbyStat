using System;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Query;
using EmbyStat.Repositories;
using FluentAssertions;
using Tests.Unit.Builders;
using Xunit;

namespace Tests.Unit.Repository
{
    public class MovieRepositoryTests : BaseRepositoryTester
    {
        private MovieRepository _movieRepository;

        public MovieRepositoryTests() : base("test-data-movie-repo.db")
        {

        }

        protected override void SetupRepository()
        {
            _movieRepository = new MovieRepository(CreateDbContext());
        }

        [Fact]
        public void UpsertRange_Should_Insert_Movies()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).Build();

                _movieRepository.UpsertRange(new[] { movieOne, movieTwo });

                var movieOneDb = _movieRepository.GetMovieById(movieOne.Id);
                movieOneDb.Should().NotBeNull();
                movieOneDb.Id.Should().Be(movieOne.Id);

                var movieTwoDb = _movieRepository.GetMovieById(movieTwo.Id);
                movieTwoDb.Should().NotBeNull();
                movieTwoDb.Id.Should().Be(movieTwo.Id);

                var count = _movieRepository.GetMediaCount(new string[0]);
                count.Should().Be(2);
            });
        }

        [Fact]
        public void GetAll_Should_Return_All_Movies()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("A").Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("B").Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo });

                var result = _movieRepository.GetAll(new string[0]);

                result.Should().NotBeNull();
                result.Count.Should().Be(2);

                result[0].Should().NotBeNull();
                result[0].Id.Should().Be(movieOne.Id);

                result[1].Should().NotBeNull();
                result[1].Id.Should().Be(movieTwo.Id);
            });
        }

        [Fact]
        public void GetAllWithImdbId_Should_Return_List()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("A").Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("B").AddImdb(string.Empty).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("C").AddCollectionId("2").Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var result = _movieRepository.GetAllWithImdbId(new string[0]);

                result.Should().NotBeNull();
                result.Count.Should().Be(2);

                result[0].Should().NotBeNull();
                result[0].Id.Should().Be(movieOne.Id);

                result[1].Should().NotBeNull();
                result[1].Id.Should().Be(movieThree.Id);
            });
        }

        [Fact]
        public void GetAllWithImdbId_And_Library_Filter_Should_Return_List()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("A").Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("B").AddImdb(string.Empty).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("C").AddCollectionId("2").Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var result = _movieRepository.GetAllWithImdbId(new[] { "1" });

                result.Should().NotBeNull();
                result.Count.Should().Be(1);

                result[0].Should().NotBeNull();
                result[0].Id.Should().Be(movieOne.Id);
            });
        }

        [Fact]
        public void Any_Should_Return_True_If_Any_Movies_Are_In_Database()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddImdb(string.Empty).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var result = _movieRepository.Any();
                result.Should().BeTrue();
            });
        }

        [Fact]
        public void Any_Should_Return_False_If_No_Movies_Are_In_Database()
        {
            RunTest(() =>
            {
                var result = _movieRepository.Any();
                result.Should().BeFalse();
            });
        }

        [Fact]
        public void GetMediaCountForPerson_Should_Return_Movie_Count_For_Person_Id()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).ReplacePersons(new ExtraPerson { Id = movieOne.People.First().Id, Type = PersonType.Actor, Name = "Test" }).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var count = _movieRepository.GetMediaCountForPerson(movieOne.People.First().Name);

                count.Should().Be(2);
            });
        }

        [Fact]
        public void GetMovieById_Should_Return_Movie_By_Id()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var movie = _movieRepository.GetMovieById(movieTwo.Id);

                movie.Should().NotBeNull();
                movie.Id.Should().Be(movieTwo.Id);
            });
        }

        [Fact]
        public void GetGenreCount_Should_Return_Distinct_Genre_Count()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddGenres("id2").Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddGenres("id3").AddCollectionId("2").Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var count = _movieRepository.GetGenreCount(new string[0]);
                count.Should().Be(3);
            });
        }

        [Fact]
        public void GetGenreCount_With_library_Filter_Should_Return_Distinct_Genre_Count()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddGenres("id2").Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddGenres("id3").AddCollectionId("2").Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var count = _movieRepository.GetGenreCount(new[] { "1" });
                count.Should().Be(2);
            });
        }

        [Fact]
        public void GetTotalRuntime_Should_Return_Total_RunTime()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var count = _movieRepository.GetTotalRuntime(new string[0]);
                count.Should().Be(360000000000);
            });
        }

        [Fact]
        public void GetTotalRuntime_With_library_Filter_Should_Return_Total_RunTime()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var count = _movieRepository.GetTotalRuntime(new[] { "1" });
                count.Should().Be(240000000000);
            });
        }

        [Fact]
        public void GetTotalDiskSize_Should_Return_Total_Disk_Size()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var count = _movieRepository.GetTotalDiskSpace(new string[0]);
                count.Should().Be(6000);
            });
        }

        [Fact]
        public void GetTotalDiskSize_With_library_Filter_Should_Return_Total_Disk_Size()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var count = _movieRepository.GetTotalDiskSpace(new[] { "1" });
                count.Should().Be(4000);
            });
        }

        [Fact]
        public void GetNewestPremieredMedia_Should_Return_Newest_Premiered_Movie()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddPremiereDate(new DateTime(1920, 1, 2)).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddPremiereDate(new DateTime(2019, 1, 2)).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var movies = _movieRepository
                    .GetNewestPremieredMedia(new string[0], 1)
                    .ToList();

                movies.Should().NotBeNull();
                movies.Count.Should().Be(1);

                movies[0].Id.Should().Be(movieThree.Id);
            });
        }

        [Fact]
        public void GetNewestPremieredMedia_With_library_Filter_Should_Return_Newest_Premiered_Movie()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddPremiereDate(new DateTime(1920, 1, 2)).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").AddPremiereDate(new DateTime(2019, 1, 2)).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var movies = _movieRepository
                    .GetNewestPremieredMedia(new[] { "1" }, 1)
                    .ToList();

                movies.Should().NotBeNull();
                movies.Count.Should().Be(1);

                movies[0].Id.Should().Be(movieOne.Id);
            });
        }

        [Fact]
        public void GetOldestPremieredMedia_Should_Return_Oldest_Premiered_Movie()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddPremiereDate(new DateTime(2019, 1, 2)).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddPremiereDate(new DateTime(1920, 1, 2)).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var movies = _movieRepository
                    .GetOldestPremieredMedia(new string[0], 1)
                    .ToList();

                movies.Should().NotBeNull();
                movies.Count.Should().Be(1);

                movies[0].Id.Should().Be(movieThree.Id);
            });
        }

        [Fact]
        public void GetOldestPremieredMedia_With_library_Filter_Should_Return_Oldest_Premiered_Movie()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddPremiereDate(new DateTime(2019, 1, 2)).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").AddPremiereDate(new DateTime(1920, 1, 2)).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var movies = _movieRepository
                    .GetOldestPremieredMedia(new[] { "1" }, 1)
                    .ToList();

                movies.Should().NotBeNull();
                movies.Count.Should().Be(1);

                movies[0].Id.Should().Be(movieOne.Id);
            });
        }

        [Fact]
        public void GetHighestRatedMedia_Should_Return_Highest_Rated_Movie()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddCommunityRating(1).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCommunityRating(9).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var movies = _movieRepository
                    .GetHighestRatedMedia(new string[0], 1)
                    .ToList();

                movies.Should().NotBeNull();
                movies.Count.Should().Be(1);

                movies[0].Id.Should().Be(movieThree.Id);
            });
        }

        [Fact]
        public void GetHighestRatedMedia_With_library_Filter_Should_Return_Highest_Rated_Movie()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddCommunityRating(1).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").AddCommunityRating(9).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var movies = _movieRepository
                    .GetHighestRatedMedia(new[] { "1" }, 1)
                    .ToList();

                movies.Should().NotBeNull();
                movies.Count.Should().Be(1);

                movies[0].Id.Should().Be(movieOne.Id);
            });
        }

        [Fact]
        public void GetLatestAddedMedia_Should_Return_Latest_Added_Movie()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddCreateDate(new DateTime(2000, 1, 1)).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").AddCreateDate(new DateTime(2019, 1, 1)).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var movies = _movieRepository
                    .GetLatestAddedMedia(new string[0], 1)
                    .ToList();

                movies.Should().NotBeNull();
                movies.Count.Should().Be(1);

                movies[0].Id.Should().Be(movieThree.Id);
            });
        }

        [Fact]
        public void GetLatestAddedMedia_With_library_Filter_Should_Return_Latest_Added_Movie()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddCreateDate(new DateTime(2000, 1, 1)).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").AddCreateDate(new DateTime(2019, 1, 1)).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var movies = _movieRepository
                    .GetLatestAddedMedia(new[] { "1" }, 1)
                    .ToList();

                movies.Should().NotBeNull();
                movies.Count.Should().Be(1);

                movies[0].Id.Should().Be(movieOne.Id);
            });
        }

        [Fact]
        public void GetLowestRatedMedia_Should_Return_Lowest_Rated_Movie()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddCommunityRating(9).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCommunityRating(1).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var movies = _movieRepository
                    .GetLowestRatedMedia(new string[0], 1)
                    .ToList();

                movies.Should().NotBeNull();
                movies.Count.Should().Be(1);

                movies[0].Id.Should().Be(movieThree.Id);
            });
        }

        [Fact]
        public void GetLowestRatedMedia_With_library_Filter_Should_Return_Lowest_Rated_Movie()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddCommunityRating(1).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").AddCommunityRating(9).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var movies = _movieRepository
                    .GetLowestRatedMedia(new[] { "1" }, 1)
                    .ToList();

                movies.Should().NotBeNull();
                movies.Count.Should().Be(1);

                movies[0].Id.Should().Be(movieTwo.Id);
            });
        }

        [Fact]
        public void GetShortestMovie_Should_Return_Shortest_Movie()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddRunTimeTicks(1, 30, 1).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddRunTimeTicks(1, 13, 1).AddCollectionId("2").Build();
                var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddRunTimeTicks(0, 3, 1).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree, movieFour });

                var movies = _movieRepository
                    .GetShortestMovie(new string[0], TimeSpan.FromMinutes(10).Ticks, 1)
                    .ToList();
                movies.Should().NotBeNull();
                movies.Count.Should().Be(1);

                movies[0].Id.Should().Be(movieThree.Id);
            });
        }

        [Fact]
        public void GetShortestMovie_With_library_Filter_Should_Return_Shortest_Movie()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddRunTimeTicks(1, 30, 1).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddRunTimeTicks(1, 13, 1).AddCollectionId("2").Build();
                var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddRunTimeTicks(0, 3, 1).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree, movieFour });

                var movies = _movieRepository
                    .GetShortestMovie(new[] { "1" }, TimeSpan.FromMinutes(10).Ticks, 1)
                    .ToList();
                movies.Should().NotBeNull();
                movies.Count.Should().Be(1);

                movies[0].Id.Should().Be(movieTwo.Id);
            });
        }

        [Fact]
        public void GetLongestMovie_Should_Return_Longest_Movie()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddRunTimeTicks(1, 30, 1).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddRunTimeTicks(13, 10, 1).AddCollectionId("2").Build();
                var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddRunTimeTicks(0, 3, 1).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree, movieFour });

                var movies = _movieRepository
                    .GetLongestMovie(new string[0], 1)
                    .ToList();
                movies.Should().NotBeNull();
                movies.Count.Should().Be(1);

                movies[0].Id.Should().Be(movieThree.Id);
            });
        }

        [Fact]
        public void GetLongestMovie_With_library_Filter_Should_Return_Longest_Movie()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddRunTimeTicks(1, 30, 1).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddRunTimeTicks(4, 13, 1).AddCollectionId("2").Build();
                var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddRunTimeTicks(0, 3, 1).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree, movieFour });

                var movies = _movieRepository
                    .GetLongestMovie(new[] { "1" }, 1)
                    .ToList();
                movies.Should().NotBeNull();
                movies.Count.Should().Be(1);

                movies[0].Id.Should().Be(movieOne.Id);
            });
        }

        [Fact]
        public void GetPeopleCount_Should_Return_Correct_Actor_Count()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).AddPerson(new ExtraPerson { Id = Guid.NewGuid().ToString(), Type = PersonType.Director, Name = "Test" }).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddPerson(new ExtraPerson { Id = movieOne.People.First().Id, Type = PersonType.Actor, Name = "Test" }).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var count = _movieRepository.GetPeopleCount(new string[0], PersonType.Actor);
                count.Should().Be(3);
            });
        }

        [Fact]
        public void GetPeopleCount_With_Library_Filter_Should_Return_Correct_Actor_Count()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).AddPerson(new ExtraPerson { Id = Guid.NewGuid().ToString(), Type = PersonType.Director, Name = "Test" }).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddPerson(new ExtraPerson { Id = movieOne.People.First().Id, Type = PersonType.Actor, Name = "Test" }).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var count = _movieRepository.GetPeopleCount(new[] { "1" }, PersonType.Actor);
                count.Should().Be(2);
            });
        }

        [Fact]
        public void GetPeopleCount_Should_Return_Correct_Director_Count()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).AddPerson(new ExtraPerson { Id = Guid.NewGuid().ToString(), Type = PersonType.Director, Name = "Test" }).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddPerson(new ExtraPerson { Id = movieOne.People.First().Id, Type = PersonType.Actor, Name = "Test" }).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var count = _movieRepository.GetPeopleCount(new string[0], PersonType.Director);
                count.Should().Be(1);
            });
        }

        [Fact]
        public void GetMostFeaturedPerson_Most_Featured_Actor_Should_Return_Correct()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddPerson(new ExtraPerson { Id = movieOne.People.First().Id, Type = PersonType.Actor, Name = "Test" }).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var people = _movieRepository.GetMostFeaturedPersons(new string[0], PersonType.Actor, 5);
                //TODO Fix test

            });
        }

        [Fact]
        public void GetToShortMovieList_Should_Return_List_With_All_To_Short_Movies()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("A").Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("B").Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("C").AddCollectionId("2").Build();
                var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("D").AddRunTimeTicks(0, 3, 1).Build();
                var movieFive = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("E").AddCollectionId("2").AddRunTimeTicks(0, 4, 1).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree, movieFour, movieFive });

                var movies = _movieRepository.GetToShortMovieList(new string[0], 10);
                movies.Should().NotBeNull();
                movies.Should().NotContainNulls();
                movies.Count.Should().Be(2);

                movies[0].Id.Should().Be(movieFour.Id);
                movies[1].Id.Should().Be(movieFive.Id);
            });
        }

        [Fact]
        public void GetToShortMovieList_With_Library_Filter_Should_Return_List_With_All_To_Short_Movies()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").Build();
                var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddRunTimeTicks(0, 3, 1).Build();
                var movieFive = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").AddRunTimeTicks(0, 4, 1).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree, movieFour, movieFive });

                var movies = _movieRepository.GetToShortMovieList(new[] { "1" }, 10);
                movies.Should().NotBeNull();
                movies.Should().NotContainNulls();
                movies.Count.Should().Be(1);

                movies[0].Id.Should().Be(movieFour.Id);
            });
        }

        [Fact]
        public void GetMoviesWithoutImdbId_Should_Return_List_With_All_Movies_Without_Imdb_Id()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("A").Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("B").Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("C").AddCollectionId("2").Build();
                var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("D").AddImdb(string.Empty).Build();
                var movieFive = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("E").AddCollectionId("2").AddImdb(string.Empty).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree, movieFour, movieFive });

                var movies = _movieRepository.GetMoviesWithoutImdbId(new string[0]);
                movies.Should().NotBeNull();
                movies.Should().NotContainNulls();
                movies.Count.Should().Be(2);

                movies[0].Id.Should().Be(movieFour.Id);
                movies[1].Id.Should().Be(movieFive.Id);
            });
        }

        [Fact]
        public void GetMoviesWithoutImdbId_Id_With_Library_Filter_Should_Return_List_With_All_Movies_Without_Imdb()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").Build();
                var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddImdb(string.Empty).Build();
                var movieFive = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").AddImdb(string.Empty).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree, movieFour, movieFive });

                var movies = _movieRepository.GetMoviesWithoutImdbId(new[] { "1" });
                movies.Should().NotBeNull();
                movies.Should().NotContainNulls();
                movies.Count.Should().Be(1);

                movies[0].Id.Should().Be(movieFour.Id);
            });
        }

        [Fact]
        public void GetMoviesWithoutPrimaryImage_Should_Return_List_With_All_Movies_Without_Primary_Image()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("A").Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("B").Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("C").AddCollectionId("2").Build();
                var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("D").AddPrimaryImage(string.Empty).Build();
                var movieFive = new MovieBuilder(Guid.NewGuid().ToString()).AddSortName("E").AddCollectionId("2").AddPrimaryImage(string.Empty).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree, movieFour, movieFive });

                var movies = _movieRepository.GetMoviesWithoutPrimaryImage(new string[0]);
                movies.Should().NotBeNull();
                movies.Should().NotContainNulls();
                movies.Count.Should().Be(2);

                movies[0].Id.Should().Be(movieFour.Id);
                movies[1].Id.Should().Be(movieFive.Id);
            });
        }

        [Fact]
        public void GetMoviesWithoutPrimaryImage_With_Library_Filter_Should_Return_List_With_All_Movies_Without_Primary_Image()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").Build();
                var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddPrimaryImage(string.Empty).Build();
                var movieFive = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").AddPrimaryImage(string.Empty).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree, movieFour, movieFive });

                var movies = _movieRepository.GetMoviesWithoutPrimaryImage(new[] { "1" });
                movies.Should().NotBeNull();
                movies.Should().NotContainNulls();
                movies.Count.Should().Be(1);

                movies[0].Id.Should().Be(movieFour.Id);
            });
        }

        [Fact]
        public void RemoveMovies_Should_Delete_All_Movies()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo });

                _movieRepository.RemoveMovies();
                var result = _movieRepository.Any();
                result.Should().BeFalse();
            });
        }

        [Fact]
        public void CalculateContainerFilterValues_Should_Return_Two_Containers()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).AddContainer("mkv").Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddContainer("mkv").Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var containers = _movieRepository
                    .CalculateContainerFilterValues(new[] { "1" })
                    .ToList();

                containers.Should().NotBeNull();
                containers.Count.Should().Be(2);

                containers[0].Label.Should().Be("avi");
                containers[0].Value.Should().Be("avi");
                containers[1].Label.Should().Be("mkv");
                containers[1].Value.Should().Be("mkv");
            });
        }

        [Fact]
        public void CalculateSubtitleFilterValues_Should_Return_Two_Languages()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).ReplaceSubtitleStream(new SubtitleStream { Language = "nl", DisplayTitle = "Dutch" }).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).ReplaceSubtitleStream(new SubtitleStream { Language = "en", DisplayTitle = "English" }).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).ReplaceSubtitleStream(new SubtitleStream { Language = "en", DisplayTitle = "English" }).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var languages = _movieRepository
                    .CalculateSubtitleFilterValues(new[] { "1" })
                    .ToList();

                languages.Should().NotBeNull();
                languages.Count.Should().Be(2);

                languages[0].Value.Should().Be("nl");
                languages[0].Label.Should().Be("Dutch");
                languages[1].Value.Should().Be("en");
                languages[1].Label.Should().Be("English");
            });
        }

        [Fact]
        public void CalculateSubtitleFilterValues_Should_Skip_Und_Languages()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).ReplaceSubtitleStream(new SubtitleStream { Language = "nl", DisplayTitle = "Dutch" }).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).ReplaceSubtitleStream(new SubtitleStream { Language = "und" }).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).ReplaceSubtitleStream(new SubtitleStream { Language = "Und" }).Build();
                var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).ReplaceSubtitleStream(new SubtitleStream { Language = null }).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree, movieFour });

                var languages = _movieRepository
                    .CalculateSubtitleFilterValues(new[] { "1" })
                    .ToList();

                languages.Should().NotBeNull();
                languages.Count.Should().Be(1);

                languages[0].Label.Should().Be("Dutch");
                languages[0].Value.Should().Be("nl");
            });
        }

        [Fact]
        public void CalculateGenreFilterValues_Should_Return_Two_Genres()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).AddGenres("Action", "SiFi").Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var genres = _movieRepository
                    .CalculateGenreFilterValues(new[] { "1" })
                    .ToList();

                genres.Should().NotBeNull();
                genres.Count.Should().Be(3);

                genres[0].Label.Should().Be("Action");
                genres[0].Value.Should().Be("Action");
                genres[1].Label.Should().Be("id1");
                genres[1].Value.Should().Be("id1");
                genres[2].Label.Should().Be("SiFi");
                genres[2].Value.Should().Be("SiFi");
            });
        }

        [Fact]
        public void CalculateCollectionFilterValues_Should_Return_Two_Genres()
        {
            RunTest(() =>
            {
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var collections = _movieRepository
                    .CalculateCollectionFilterValues()
                    .ToList();

                collections.Should().NotBeNull();
                collections.Count.Should().Be(2);

                collections[0].Label.Should().Be("1");
                collections[0].Value.Should().Be("1");
                collections[1].Label.Should().Be("2");
                collections[1].Value.Should().Be("2");
            });
        }

        [Theory]
        [InlineData("Container", "mkv", "==", 1)]
        [InlineData("Container", "mkv", "!=", 4)]
        [InlineData("Container", "", "null", 1)]
        [InlineData("Subtitles", "nl", "any", 1)]
        [InlineData("Subtitles", "en", "!any", 2)]
        [InlineData("Subtitles", "en", "empty", 1)]
        [InlineData("SizeInMb", "1", "<", 2)]
        [InlineData("SizeInMb", "1", ">", 3)]
        [InlineData("SizeInMb", "1", "null", 1)]
        [InlineData("SizeInMb", "3|4", "between", 1)]
        [InlineData("Height", "1000", "==", 2)]
        [InlineData("Height", "900", "<", 1)]
        [InlineData("Height", "1100", ">", 1)]
        [InlineData("Height", "1100", "null", 1)]
        [InlineData("Height", "1100|2000", "between", 1)]
        [InlineData("Width", "1000", "==", 2)]
        [InlineData("Width", "900", "<", 1)]
        [InlineData("Width", "1100", ">", 1)]
        [InlineData("Width", "1100", "null", 1)]
        [InlineData("Width", "1100|2000", "between", 1)]
        [InlineData("AverageFrameRate", "25", "==", 2)]
        [InlineData("AverageFrameRate", "20", "<", 1)]
        [InlineData("AverageFrameRate", "40", ">", 1)]
        [InlineData("AverageFrameRate", "40", "null", 1)]
        [InlineData("AverageFrameRate", "40|60", "between", 1)]
        [InlineData("Genres", "id1", "any", 4)]
        [InlineData("Genres", "id1", "!any", 1)]
        [InlineData("Images", "Primary", "!null", 4)]
        [InlineData("Images", "Primary", "null", 1)]
        [InlineData("Images", "Logo", "!null", 4)]
        [InlineData("Images", "Logo", "null", 1)]
        [InlineData("CommunityRating", "8", "==", 1)]
        [InlineData("CommunityRating", "7|9", "between", 1)]
        [InlineData("RunTimeTicks", "100000000000", "<", 1)]
        [InlineData("RunTimeTicks", "100000000000", ">", 4)]
        [InlineData("RunTimeTicks", "100000000000|10000000", "between", 1)]
        [InlineData("Name", "The Hobbit", "==", 1)]
        [InlineData("Name", "The Hobbit", "!=", 4)]
        [InlineData("Name", "The", "contains", 5)]
        [InlineData("Name", "Hobbit", "!contains", 4)]
        [InlineData("Name", "The", "startsWith", 5)]
        [InlineData("Name", "rings", "endsWith", 4)]
        [InlineData("IMDB", "", "null", 1)]
        [InlineData("PremiereDate", "1/01/2002 0:00:00", "==", 2)]
        [InlineData("PremiereDate", "1/01/1000 0:00:00", "<", 1)]
        [InlineData("PremiereDate", "1/01/1000 0:00:00", ">", 2)]
        [InlineData("PremiereDate", "1/01/900 0:00:00|1/01/1500 0:00:00", "between", 1)]
        [InlineData("PremiereDate", "", "null", 1)]
        [InlineData("BitDepth", "9", "<", 3)]
        [InlineData("BitDepth", "9", ">", 1)]
        [InlineData("BitDepth", "0", "null", 1)]
        [InlineData("BitDepth", "2|4", "between", 1)]
        [InlineData("Codec", "h264", "any", 2)]
        [InlineData("Codec", "h264", "!any", 3)]
        [InlineData("VideoRange", "SDR", "any", 3)]
        [InlineData("VideoRange", "SDR", "!any", 2)]
        public void GetMediaCount_Should_Filter(string field, string value, string operation, int result)
        {
            RunTest(() =>
            {
                var filters = new[] { new Filter { Field = field, Value = value, Operation = operation } };
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString())
                    .AddContainer("mkv")
                    .AddName("The Hobbit")
                    .AddImdb(null)
                    .AddPremiereDate(new DateTime(1000, 1, 1))
                    .ReplaceMediaSources(new MediaSource { SizeInMb = 100 })
                    .Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString())
                    .ReplaceMediaSources(new MediaSource { SizeInMb = 0.01 })
                    .AddPremiereDate(null)
                    .ReplaceVideoStreams(
                        new VideoStreamBuilder()
                            .AddWidth(1200)
                            .AddHeight(1200)
                            .AddBitDepth(3)
                            .AddCodec("h265")
                            .AddVideoRange("HDR")
                            .AddAverageFrameRate(50)
                            .Build())
                    .Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString())
                    .ReplaceMediaSources(new MediaSource { SizeInMb = 3500 })
                    .AddPremiereDate(new DateTime(100, 1, 1))
                    .ReplaceVideoStreams(
                        new VideoStreamBuilder()
                            .AddWidth(100)
                            .AddHeight(100)
                            .AddCodec("h265")
                            .AddVideoRange("HDR")
                            .AddBitDepth(null)
                            .AddAverageFrameRate(3)
                            .Build())
                    .AddGenres("id2")
                    .AddPrimaryImage(null)
                    .AddLogo(null)
                    .EmptySubtitleStreams()
                    .AddCommunityRating(8f)
                    .Build();
                var movieFour = new MovieBuilder(Guid.NewGuid().ToString())
                    .AddContainer(null)
                    .ReplaceSubtitleStream(new SubtitleStreamBuilder("nl").Build())
                    .AddCommunityRating(7f)
                    .AddRunTimeTicks(1, 0, 0)
                    .Build();
                var movieFive = new MovieBuilder(Guid.NewGuid().ToString())
                    .ReplaceVideoStreams(
                        new VideoStreamBuilder()
                            .AddWidth(null)
                            .AddHeight(null)
                            .AddBitDepth(10)
                            .AddCodec("h265")
                            .AddAverageFrameRate(null)
                            .Build())
                    .AddCommunityRating(9f)
                    .Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree, movieFour, movieFive });

                var queryResult = _movieRepository.GetMediaCount(filters, new string[0]);
                queryResult.Should().Be(result);
            });
        }
    }
}
