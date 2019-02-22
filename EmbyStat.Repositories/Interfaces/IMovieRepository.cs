using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        void RemoveMovies();
        void AddOrUpdate(Movie movie);
        int GetTotalPersonByType(IEnumerable<string> collections, string type);
        string GetMostFeaturedPerson(IEnumerable<string> collections, string type);
        List<Movie> GetAll(IEnumerable<string> collections, bool includeSubs = false);
        List<string> GetGenres(IEnumerable<string> collections);
        bool Any();
        int GetMovieCountForPerson(string personId);
        Movie GetMovieById(string id);
    }
}
