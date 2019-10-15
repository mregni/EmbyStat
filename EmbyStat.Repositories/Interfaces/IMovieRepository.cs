using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        void RemoveMovies();
        void UpsertRange(IEnumerable<Movie> movies);
        List<Movie> GetAll(IReadOnlyList<string> collections);
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
    }
}
