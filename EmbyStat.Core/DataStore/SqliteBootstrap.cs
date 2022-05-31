using EmbyStat.Configuration.Interfaces;
using Microsoft.Data.Sqlite;

namespace EmbyStat.Core.DataStore;

public class SqliteBootstrap : ISqliteBootstrap
{
    private readonly string _connectionString;
    public SqliteBootstrap(IConfigurationService configurationService)
    {
        var config = configurationService.Get();
        var path = Path.Combine(config.SystemConfig.Dirs.Data, "SqliteData.db");
        _connectionString = $"Data Source={path}";
    }


    public SqliteConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}