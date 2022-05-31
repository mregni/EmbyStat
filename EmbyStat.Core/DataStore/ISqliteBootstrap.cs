using Microsoft.Data.Sqlite;

namespace EmbyStat.Core.DataStore;

public interface ISqliteBootstrap
{
    SqliteConnection CreateConnection();
}