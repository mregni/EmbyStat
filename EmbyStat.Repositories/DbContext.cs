using System;
using System.IO;
using AspNetCore.Identity.LiteDB.Data;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using LiteDB;
using Microsoft.Extensions.Options;

namespace EmbyStat.Repositories
{
    public class DbContext : IDbContext, ILiteDbContext
    {
        private readonly AppSettings _settings;

        public DbContext(IOptions<AppSettings> settings)
        {
            _settings = settings.Value;
        }

        public LiteDatabase CreateDatabaseContext()
        {
            try
            {
                var dbPath = Path.Combine(_settings.Dirs.Data, _settings.DatabaseFile);
                var database = new LiteDatabase($"FileName={dbPath}; Connection=shared");
                database.Mapper.EnumAsInteger = true;
                return database;
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException("Can find or create LiteDb database.", ex);
            }
        }

        public ILiteDatabase LiteDatabase => CreateDatabaseContext();
    }
}
