using System.Collections.Generic;
using System.Linq;
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
    }
}
