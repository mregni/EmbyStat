using EmbyStat.Common.Models.DataGrid;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Query;

namespace EmbyStat.Core.Shows.Interfaces;

public interface IShowService
{
    Task<List<Library>> GetShowLibraries();
    Task<ShowStatistics> GetStatistics();
    Task<ShowStatistics> CalculateShowStatistics();
    bool TypeIsPresent();
    Task<Page<Show>> GetShowPage(int skip, int take, string sortField, string sortOrder, Filter[] filters, bool requireTotalCount);
    Task<Show> GetShow(string id);
    Task SetLibraryAsSynced(string[] libraryIds);
}