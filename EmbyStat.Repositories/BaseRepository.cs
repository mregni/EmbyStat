using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Repositories.Interfaces;
using LiteDB;

namespace EmbyStat.Repositories
{
    public abstract class BaseRepository : IDisposable
    {
        internal readonly IDbContext Context;

        protected BaseRepository(IDbContext context)
        {
            Context = context;
        }

        public T1 ExecuteQuery<T1>(Func<T1> query)
        {
            var result = query();
            GCCollect();
            return result;
        }

        public void ExecuteQuery(Action query)
        {
            query();
            GCCollect();
        }

        public void Dispose()
        {
            GCCollect();
        }

        internal static IEnumerable<T> GetWorkingLibrarySet<T>(LiteCollection<T> collection, IReadOnlyList<string> libraryIds)
        {
            return libraryIds.Any()
                ? collection.Find(Query.In("CollectionId", libraryIds.ConvertToBsonArray()))
                : collection.FindAll();
        }

        private void GCCollect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
