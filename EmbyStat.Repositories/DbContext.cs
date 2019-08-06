using System;
using System.IO;
using EmbyStat.Common.Extensions;
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
                var dbPath = Path.Combine(settings.Value.Dirs.Config, settings.Value.DatabaseFile).GetLocalPath();
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
