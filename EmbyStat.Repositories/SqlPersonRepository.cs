using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.SqLite;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories
{
    public class SqlPersonRepository : IPersonRepository
    {
        private readonly ISqliteBootstrap _sqliteBootstrap;
        private readonly SqlLiteDbContext _context;

        public SqlPersonRepository(SqlLiteDbContext context, ISqliteBootstrap sqliteBootstrap)
        {
            _context = context;
            _sqliteBootstrap = sqliteBootstrap;
        }

        public async Task UpsertRange(IEnumerable<SqlPerson> people)
        {
            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();
            await using var transaction = connection.BeginTransaction();

            var query = $"INSERT OR IGNORE INTO {Constants.Tables.People} (Id,Name,BirthDate,\"Primary\") VALUES (@Id, @Name, @BirthDate, @Primary)";
            await connection.ExecuteAsync(query, people, transaction);
            await transaction.CommitAsync();
        }

        public async Task DeleteAll()
        {
            _context.People.RemoveRange(_context.People);
            await _context.SaveChangesAsync();
        }
    }
}
