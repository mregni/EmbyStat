using Dapper;
using EmbyStat.Common;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Core.DataStore;
using EmbyStat.Core.Genres.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Core.Genres;

public class GenreRepository : IGenreRepository
{
    private readonly ISqliteBootstrap _sqliteBootstrap;
    private readonly EsDbContext _context;

    public GenreRepository(ISqliteBootstrap sqliteBootstrap, EsDbContext context)
    {
        _sqliteBootstrap = sqliteBootstrap;
        _context = context;
    }

    public async Task UpsertRange(IEnumerable<Genre> genres)
    {
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();
        await using var transaction = connection.BeginTransaction();

        var query = @$"INSERT OR IGNORE INTO {Constants.Tables.Genres} (Id, Name) VALUES (@Id, @Name)";
        await connection.ExecuteAsync(query, genres, transaction);

        await transaction.CommitAsync();
    }

    public Task<Genre[]> GetAll()
    {
        return _context.Genres.ToArrayAsync();
    }

    public async Task DeleteAll()
    {
        _context.Genres.RemoveRange(_context.Genres);
        await _context.SaveChangesAsync();
    }
}