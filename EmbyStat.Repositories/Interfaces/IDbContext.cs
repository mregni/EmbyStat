using LiteDB;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IDbContext
    {
        LiteDatabase CreateDatabaseContext();
    }
}
