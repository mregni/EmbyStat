using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using EmbyStat.Common;
using EmbyStat.Common.SqLite;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly ISqliteBootstrap _sqliteBootstrap;
        private readonly DbContext _context;

        public GenreRepository(ISqliteBootstrap sqliteBootstrap, DbContext context)
        {
            _sqliteBootstrap = sqliteBootstrap;
            _context = context;
        }

        public async Task UpsertRange(IEnumerable<SqlGenre> genres)
        {
            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();
            await using var transaction = connection.BeginTransaction();

            var query = @$"INSERT OR IGNORE INTO {Constants.Tables.Genres} (Id, Name) VALUES (@Id, @Name)";
            await connection.ExecuteAsync(query, genres, transaction);

            await transaction.CommitAsync();
        }

        public Task<SqlGenre[]> GetAll()
        {
            return _context.Genres.ToArrayAsync();
        }

        public async Task DeleteAll()
        {
            _context.Genres.RemoveRange(_context.Genres);
            await _context.SaveChangesAsync();
        }
    }
}