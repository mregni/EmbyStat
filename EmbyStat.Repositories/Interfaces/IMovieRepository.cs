using System.Collections.Generic;
using EmbyStat.Common.Models;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        void RemoveMovies();
        void Add(Movie movie);
        int GetTotalPersonByType(List<string> collections, string type);
        string GetMostFeaturedPerson(List<string> collections, string type);
        List<Movie> GetAll(IEnumerable<string> collections, bool inludeSubs = false);
        List<string> GetGenres(List<string> collections);
        bool Any();
    }
}
