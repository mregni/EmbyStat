using Dapper;

namespace EmbyStat.Core.DataStore;

public static class SqliteBootstrapExtensions
{
    public static async Task ExecuteQuery(this ISqliteBootstrap sqliteBootstrap, string query, object param)
    {
        await using var connection = sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();
        await using var transaction = connection.BeginTransaction(); 
        await connection.ExecuteAsync(query, param, transaction);
        await transaction.CommitAsync();
    }  
}