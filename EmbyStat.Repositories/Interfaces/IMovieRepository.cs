using EmbyStat.Common.Models.Entities;
using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Repositories.Interfaces.Helpers;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMovieRepository : IMediaRepository<Movie>
    {
        void RemoveMovies();
        void UpsertRange(IEnumerable<Movie> movies);
        List<Movie> GetAll(IReadOnlyList<string> libraryIds);
        List<Movie> GetAllWithImdbId(IReadOnlyList<string> libraryIds);
        Movie GetMovieById(string id);
        int GetGenreCount(IReadOnlyList<string> libraryIds);
        long GetTotalRuntime(IReadOnlyList<string> libraryIds);
        Movie GetShortestMovie(IReadOnlyList<string> libraryIds, long toShortMovieTicks);
        Movie GetLongestMovie(IReadOnlyList<string> libraryIds);
        double GetTotalDiskSize(IReadOnlyList<string> libraryIds);
        int GetPeopleCount(IReadOnlyList<string> libraryIds, PersonType type);
        string GetMostFeaturedPerson(IReadOnlyList<string> libraryIds, PersonType type);
        List<Movie> GetToShortMovieList(IReadOnlyList<string> libraryIds, int toShortMovieMinutes);
        List<Movie> GetMoviesWithoutImdbId(IReadOnlyList<string> libraryIds);
        List<Movie> GetMoviesWithoutPrimaryImage(IReadOnlyList<string> libraryIds);
    }
}
