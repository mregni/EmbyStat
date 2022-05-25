using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Core.Filters.Interfaces;

public interface IFilterRepository
{
    Task<FilterValues> Get(LibraryType type, string field);
    Task Insert(FilterValues values);
    Task DeleteAll(LibraryType type);
    Task DeleteAll();
}