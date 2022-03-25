using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class FilterRepository : BaseRepository, IFilterRepository
    {
        public FilterRepository(IDbContext context) : base(context)
        {

        }

        public FilterValues Get(string field)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<FilterValues>("Filters");
            return collection
                .Find(x => x.Field == field)
                .FirstOrDefault();
        }

        public void Insert(FilterValues values)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<FilterValues>("Filters");
            collection.Insert(values);
        }
    }
}
