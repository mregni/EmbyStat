using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common.Extensions;
using EmbyStat.Repositories.Interfaces;
using LiteDB;

namespace EmbyStat.Repositories
{
    public abstract class BaseRepository
    {
        internal readonly IDbContext Context;

        protected BaseRepository(IDbContext context)
        {
            Context = context;
        }

        internal static IEnumerable<T> GetWorkingLibrarySet<T>(ILiteCollection<T> collection, IReadOnlyList<string> libraryIds)
        {
            return libraryIds.Any()
                ? collection.Find(Query.In("CollectionId", libraryIds.ConvertToBsonArray()))
                : collection.FindAll();
        }
    }
}
