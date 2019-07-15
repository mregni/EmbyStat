using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        void RemoveMovies();
        void UpsertRange(IEnumerable<Movie> movies);
        IEnumerable<Movie> GetAll(IEnumerable<string> collections);
        bool Any();
        int GetMovieCountForPerson(string personId);
        Movie GetMovieById(string id);
    }
}
