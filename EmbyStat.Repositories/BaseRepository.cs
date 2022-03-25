using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public abstract class BaseRepository
    {
        internal readonly IDbContext Context;

        protected BaseRepository(IDbContext context)
        {
            Context = context;
        }
    }
}
