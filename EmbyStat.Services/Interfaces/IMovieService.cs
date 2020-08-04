using System.Collections.Generic;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Query;
using EmbyStat.Services.Models.DataGrid;
using EmbyStat.Services.Models.Movie;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmbyStat.Services.Interfaces
{
    public interface IMovieService
    {
        IEnumerable<Library> GetMovieLibraries();
        MovieStatistics GetStatistics(List<string> libraryIds);
        MovieStatistics CalculateMovieStatistics(List<string> libraryIds);
        MovieStatistics CalculateMovieStatistics(string libraryId);
        bool TypeIsPresent();
        Page<MovieColumn> GetMoviePage(int skip, int take, string sort, Filter[] filters, bool requireTotalCount, List<string> libraryIds);
        Movie GetMovie(string id);
    }
}
