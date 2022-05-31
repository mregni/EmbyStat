using EmbyStat.Common.Models.DataGrid;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Query;

namespace EmbyStat.Core.Movies.Interfaces;

public interface IMovieService
{
    Task<List<Library>> GetMovieLibraries();
    Task<MovieStatistics> GetStatistics();
    Task<MovieStatistics> CalculateMovieStatistics();
    bool TypeIsPresent();
    Task<Page<Movie>> GetMoviePage(int skip, int take, string sortField, string sortOrder, Filter[] filters, bool requireTotalCount);
    Task<Movie> GetMovie(string id);
    Task SetLibraryAsSynced(string[] libraryIds);
}