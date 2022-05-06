using EmbyStat.Repositories.Interfaces;
using Microsoft.Data.Sqlite;

namespace EmbyStat.Repositories;

public class SqliteBootstrap : ISqliteBootstrap
{
    private readonly string _connectionString;
    public SqliteBootstrap()
    {
        _connectionString = "Data Source=SqliteData.db";
    }


    public SqliteConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}