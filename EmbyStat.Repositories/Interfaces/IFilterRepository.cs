using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IFilterRepository
    {
        FilterValues Get(string field, string[] libraryIds);
        void Insert(FilterValues values);
    }
}
