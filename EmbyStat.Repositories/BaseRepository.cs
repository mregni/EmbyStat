using System;

namespace EmbyStat.Repositories
{
    public abstract class BaseRepository : IDisposable
    {
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

        private void GCCollect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
