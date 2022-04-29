using Microsoft.Data.Sqlite;

namespace EmbyStat.Repositories.Interfaces;

public interface ISqliteBootstrap
{
    SqliteConnection CreateConnection();
}