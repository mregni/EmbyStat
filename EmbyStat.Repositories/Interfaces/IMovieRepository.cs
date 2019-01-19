using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;
using MediaBrowser.Model.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        void RemoveMovies();
        void AddOrUpdate(Movie movie);
        int GetTotalPersonByType(List<string> collections, PersonType type);
        string GetMostFeaturedPerson(List<string> collections, PersonType type);
        List<Movie> GetAll(IEnumerable<string> collections, bool includeSubs = false);
        List<string> GetGenres(List<string> collections);
        bool Any();
        int GetMovieCountForPerson(string personId);
    }
}
