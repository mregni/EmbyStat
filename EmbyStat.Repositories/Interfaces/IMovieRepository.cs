using EmbyStat.Common.Models.Entities;
using MediaBrowser.Model.Entities;
using System.Collections.Generic;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        void RemoveMovies();
        void UpsertRange(IEnumerable<Movie> movies);
        List<Movie> GetAll(IReadOnlyList<string> collections);
        List<Movie> GetAllWithImdbId(IReadOnlyList<string> libraryIds);
        bool Any();
        int GetMovieCountForPerson(string personId);
        Movie GetMovieById(string id);
        int GetMovieCount(IReadOnlyList<string> libraryIds);
        int GetGenreCount(IReadOnlyList<string> libraryIds);
        Movie GetHighestRatedMovie(IReadOnlyList<string> libraryIds);
        long GetTotalRuntime(IReadOnlyList<string> libraryIds);
        Movie GetLowestRatedMovie(IReadOnlyList<string> libraryIds);
        Movie GetOldestPremiered(IReadOnlyList<string> libraryIds);
        Movie GetNewestPremiered(IReadOnlyList<string> libraryIds);
        Movie GetShortestMovie(IReadOnlyList<string> libraryIds, long toShortMovieTicks);
        Movie GetLongestMovie(IReadOnlyList<string> libraryIds);
        Movie GetLatestAdded(IReadOnlyList<string> libraryIds);
        double GetTotalDiskSize(IReadOnlyList<string> libraryIds);
        int GetPeopleCount(IReadOnlyList<string> libraryIds, PersonType type);
        string GetMostFeaturedPerson(IReadOnlyList<string> libraryIds, PersonType type);
        List<Movie> GetToShortMovieList(IReadOnlyList<string> libraryIds, int toShortMovieMinutes);
        List<Movie> GetMoviesWithoutImdbId(IReadOnlyList<string> libraryIds);
        List<Movie> GetMoviesWithoutPrimaryImage(IReadOnlyList<string> libraryIds);
    }
}
