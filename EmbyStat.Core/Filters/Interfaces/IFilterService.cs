using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Core.Filters.Interfaces;

public interface IFilterService
{
    Task<FilterValues> GetFilterValues(LibraryType type, string field);
    FilterValues CalculateFilterValues(LibraryType type, string field);
}