﻿using Dapper;
using EmbyStat.Common;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Core.DataStore;
using EmbyStat.Core.People.Interfaces;

namespace EmbyStat.Core.People;

public class PersonRepository : IPersonRepository
{
    private readonly ISqliteBootstrap _sqliteBootstrap;
    private readonly EsDbContext _context;

    public PersonRepository(EsDbContext context, ISqliteBootstrap sqliteBootstrap)
    {
        _context = context;
        _sqliteBootstrap = sqliteBootstrap;
    }

    public async Task UpsertRange(IEnumerable<Person> people)
    {
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();
        await using var transaction = connection.BeginTransaction();

        var query = $"INSERT OR IGNORE INTO {Constants.Tables.People} (Id,Name,\"Primary\") VALUES (@Id, @Name, @Primary)";
        await connection.ExecuteAsync(query, people, transaction);
        await transaction.CommitAsync();
    }

    public async Task DeleteAll()
    {
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();
        await using var transaction = connection.BeginTransaction();

        var query = $"DELETE FROM {Constants.Tables.People}";
        await connection.ExecuteAsync(query, transaction);
        await transaction.CommitAsync();
        
        _context.People.RemoveRange(_context.People);
        await _context.SaveChangesAsync();
    }
}