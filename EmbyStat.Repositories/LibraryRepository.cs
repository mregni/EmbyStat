using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using LiteDB;

namespace EmbyStat.Repositories
{
    public class LibraryRepository : BaseRepository, ILibraryRepository
    {
        private readonly LiteCollection<Library> _libraryCollection;

        public LibraryRepository(IDbContext context) : base(context)
        {
            _libraryCollection = context.GetContext().GetCollection<Library>();
        }

        public List<Library> GetLibrariesByTypes(IEnumerable<LibraryType> types)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Library>();
                    return collection.Find(Query.In("Type", types.ConvertToBsonArray())).OrderBy(x => x.Name).ToList();
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
