using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EmbyStat.Common;
using EmbyStat.Common.SqLite;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly ISqliteBootstrap _sqliteBootstrap;

        public GenreRepository(ISqliteBootstrap sqliteBootstrap)
        {
            _sqliteBootstrap = sqliteBootstrap;
        }

        public async Task UpsertRange(IEnumerable<SqlGenre> genres)
        {
            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();
            await using var transaction = connection.BeginTransaction();

            var query = @$"INSERT OR REPLACE INTO {Constants.Tables.Genres} (Id, Name) VALUES (@Id, @Name)";
            await connection.ExecuteAsync(query, genres, transaction);

            await transaction.CommitAsync();
        }

        public async Task<SqlGenre[]> GetAll()
        {
            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();
            await using var transaction = connection.BeginTransaction();

            var query = $"SELECT * FROM {Constants.Tables.Genres}";
            var result = await connection.QueryAsync<SqlGenre>(query);
            return result.ToArray();
        }
    }
}
