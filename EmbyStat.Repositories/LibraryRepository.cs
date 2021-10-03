using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class LibraryRepository : BaseRepository, ILibraryRepository
    {
        public LibraryRepository(IDbContext context) : base(context)
        {
            
        }

        public List<Library> GetLibrariesById(IEnumerable<string> ids)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Library>();
            return collection
                .Find(x => ids.Contains(x.Id))
                .OrderBy(x => x.Name)
                .ToList();
        }

        public void AddOrUpdateRange(IEnumerable<Library> collections)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Library>();
            collection.Upsert(collections);
        }
    }
}
