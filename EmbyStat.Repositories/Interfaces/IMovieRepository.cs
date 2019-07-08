using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        void RemoveMovies();
        void UpsertRange(IEnumerable<Movie> movies);
        int GetTotalPeopleByType(IEnumerable<string> collectionIds, string type);
        string GetMostFeaturedPerson(IEnumerable<string> collectionIds, string type);
        IEnumerable<Movie> GetAll(IEnumerable<string> collections);
        IEnumerable<string> GetGenres(IEnumerable<string> collectionIds);
        bool Any();
        int GetMovieCountForPerson(string personId);
        Movie GetMovieById(string id);
    }
}
