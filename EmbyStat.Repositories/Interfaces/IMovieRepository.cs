using System.Collections.Generic;
using EmbyStat.Common.Models;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        void RemoveMovies();
        void Add(Movie movie);
    }
}
