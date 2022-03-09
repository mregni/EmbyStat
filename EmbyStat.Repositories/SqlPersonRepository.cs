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

        public void Upsert(Person person)
        {
            throw new NotImplementedException();
        }

        public Person GetPersonByName(string name)
        {
            throw new NotImplementedException();
        }

        public int GetMovieCountForPerson(string name, string genre)
        {
            return _context.Movies
                .Include(x => x.Genres)
                .Include(x => x.People)
                .ThenInclude(x => x.Person)
                .Count(x => x.Genres.Any(y => y.Name == genre) && x.People.Any(y => name == y.Person.Name));
        }

        public int GetMovieCountForPerson(string name)
        {
            return _context.Movies
                .Include(x => x.People)
                .ThenInclude(x => x.Person)
                .Count(x => x.People.Any(y => name == y.Person.Name));
        }

        public int GetMoviePeopleCountForType(IReadOnlyList<string> libraryIds, PersonType type)
        {
            return _context.Movies
                .Include(x => x.People)
                .FilterOnLibrary(libraryIds)
                .SelectMany(x => x.People)
                .Distinct()
                .Count(x => x.Type == type);
        }

        public IEnumerable<string> GetMovieMostFeaturedPersons(IReadOnlyList<string> libraryIds, PersonType type, int count)
        {
            return _context.Movies
                .Include(x => x.People)
                .ThenInclude(x => x.Person)
                .FilterOnLibrary(libraryIds)
                .SelectMany(x => x.People)
                .Where(x => x.Type == type)
                .GroupBy(x => x.Person.Name, (name, people) => new { Name = name, Count = people.Count() })
                .OrderByDescending(x => x.Count)
                .Select(x => x.Name)
                .Take(count);
        }
    }
}
