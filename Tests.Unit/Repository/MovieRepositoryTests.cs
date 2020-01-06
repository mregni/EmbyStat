using System;
using System.Linq;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Repositories;
using FluentAssertions;
using MediaBrowser.Model.Entities;
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
                var movieOne = new MovieBuilder(Guid.NewGuid().ToString()).Build();
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddImdb(string.Empty).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").Build();
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
                var movieTwo = new MovieBuilder(Guid.NewGuid().ToString()).AddPerson(new ExtraPerson { Id = movieOne.People.First().Id, Type = PersonType.Actor, Name = "Test" }).Build();
                var movieThree = new MovieBuilder(Guid.NewGuid().ToString()).AddCollectionId("2").Build();
                _movieRepository.UpsertRange(new[] { movieOne, movieTwo, movieThree });

                var count = _movieRepository.GetMediaCountForPerson(movieOne.People.First().Id);

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

                var count = _movieRepository.GetTotalDiskSize(new string[0]);
                count.Should().Be(3003);
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

                var count = _movieRepository.GetTotalDiskSize(new[] { "1" });
                count.Should().Be(2002);
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

                var movie = _movieRepository.GetNewestPremieredMedia(new string[0]);

                movie.Should().NotBeNull();
                movie.Id.Should().Be(movieThree.Id);
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

                var movie = _movieRepository.GetNewestPremieredMedia(new[] { "1" });

                movie.Should().NotBeNull();
                movie.Id.Should().Be(movieOne.Id);
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

                var movie = _movieRepository.GetOldestPremieredMedia(new string[0]);

                movie.Should().NotBeNull();
                movie.Id.Should().Be(movieThree.Id);
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

                var movie = _movieRepository.GetOldestPremieredMedia(new[] { "1" });

                movie.Should().NotBeNull();
                movie.Id.Should().Be(movieOne.Id);
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

                var movie = _movieRepository.GetHighestRatedMedia(new string[0]);

                movie.Should().NotBeNull();
                movie.Id.Should().Be(movieThree.Id);
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

                var movie = _movieRepository.GetHighestRatedMedia(new[] { "1" });

                movie.Should().NotBeNull();
                movie.Id.Should().Be(movieOne.Id);
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

                var movie = _movieRepository.GetLatestAddedMedia(new string[0]);

                movie.Should().NotBeNull();
                movie.Id.Should().Be(movieThree.Id);
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

                var movie = _movieRepository.GetLatestAddedMedia(new[] { "1" });

                movie.Should().NotBeNull();
                movie.Id.Should().Be(movieOne.Id);
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

                var movie = _movieRepository.GetLowestRatedMedia(new string[0]);

                movie.Should().NotBeNull();
                movie.Id.Should().Be(movieThree.Id);
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

                var movie = _movieRepository.GetLowestRatedMedia(new[] { "1" });

                movie.Should().NotBeNull();
                movie.Id.Should().Be(movieTwo.Id);
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

                var movie = _movieRepository.GetShortestMovie(new string[0], TimeSpan.FromMinutes(10).Ticks);
                movie.Should().NotBeNull();
                movie.Id.Should().Be(movieThree.Id);
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

                var movie = _movieRepository.GetShortestMovie(new[] { "1" }, TimeSpan.FromMinutes(10).Ticks);
                movie.Should().NotBeNull();
                movie.Id.Should().Be(movieTwo.Id);
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

                var movie = _movieRepository.GetLongestMovie(new string[0]);
                movie.Should().NotBeNull();
                movie.Id.Should().Be(movieThree.Id);
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

                var movie = _movieRepository.GetLongestMovie(new[] { "1" });
                movie.Should().NotBeNull();
                movie.Id.Should().Be(movieOne.Id);
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

                var person = _movieRepository.GetMostFeaturedPerson(new string[0], PersonType.Actor);
                person.Should().NotBeNullOrWhiteSpace();
                person.Should().Be("Gimli");
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
    }
}
