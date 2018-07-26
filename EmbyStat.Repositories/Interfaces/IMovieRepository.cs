using System;
using System.Collections.Generic;
using EmbyStat.Common.Models;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        void RemoveMovies();
        void AddOrUpdate(Movie movie);
        int GetTotalPersonByType(List<Guid> collections, string type);
        Guid GetMostFeaturedPerson(List<Guid> collections, string type);
        List<Movie> GetAll(IEnumerable<Guid> collections, bool inludeSubs = false);
        List<Guid> GetGenres(List<Guid> collections);
        bool Any();
    }
}
