using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Query;
using EmbyStat.Common.SqLite.Movies;
using EmbyStat.Services.Models.DataGrid;
using EmbyStat.Services.Models.Movie;

namespace EmbyStat.Services.Interfaces
{
    public interface IMovieService
    {
        IEnumerable<Library> GetMovieLibraries();
        Task<MovieStatistics> GetStatistics(List<string> libraryIds);
        Task<MovieStatistics> CalculateMovieStatistics(List<string> libraryIds);
        Task<MovieStatistics> CalculateMovieStatistics(string libraryId);
        bool TypeIsPresent();
        Task<Page<SqlMovie>> GetMoviePage(int skip, int take, string sortField, string sortOrder, Filter[] filters, bool requireTotalCount, List<string> libraryIds);
        SqlMovie GetMovie(string id);
    }
}
