using System.Collections.Generic;
using EmbyStat.Common.Models;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        void RemoveMovies();
        void Add(Movie movie);
        int GetMovieCount(List<string> collections);
        int GetGenreCount(List<string> collections);
        long GetPlayLength(List<string> collections);
        Movie GetHighestRatedMovie(List<string> collections);
        Movie GetLowestRatedMovie(List<string> collections);
        Movie GetOlderPremieredMovie(List<string> collections);
        Movie GetYoungestPremieredMovie(List<string> collections);
        Movie GetShortestMovie(List<string> collections);
        Movie GetLongestMovie(List<string> collections);
        Movie GetYoungestAddedMovie(List<string> collections);
    }
}
