using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class LibraryRepository : BaseRepository, ILibraryRepository
    {
        public LibraryRepository(IDbContext context) : base(context)
        {
            
        }

        public List<Library> GetLibrariesByTypes(IEnumerable<LibraryType> types)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Library>();
                    return collection
                        .Find(x => types.Contains(x.Type))
                        .OrderBy(x => x.Name)
                        .ToList();
                }
            });
        }

        public void AddOrUpdateRange(IEnumerable<Library> collections)
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Library>();
                    collection.Upsert(collections);
                }
            });
        }
    }
}
