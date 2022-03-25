using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IFilterRepository
    {
        FilterValues Get(string field);
        void Insert(FilterValues values);
    }
}
