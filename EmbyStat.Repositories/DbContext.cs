using System;
using System.IO;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using LiteDB;
using Microsoft.Extensions.Options;

namespace EmbyStat.Repositories
{
    public class DbContext : IDbContext
    {
        private readonly LiteDatabase _context;

        public DbContext(IOptions<AppSettings> settings)
        {
            try
            {
                var dbPath = Path.Combine(Directory.GetCurrentDirectory(), settings.Value.Dirs.Database, settings.Value.DatabaseFile);
                _context = new LiteDatabase(dbPath);
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException("Can find or create LiteDb database.", ex);
            }
        }

        public LiteDatabase GetContext()
        {
            return _context;
        }
    }
}
