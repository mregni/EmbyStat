using EmbyStat.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EmbyStat.Web
{
    public class DatabaseFactory : IDesignTimeDbContextFactory<SqlLiteDbContext>
    {
        public SqlLiteDbContext CreateDbContext(string[] args)
        {
            //var host = "localhost";
            //var port = "5432";
            //var user = "postgres";
            //var password = "28b19ad684884ccc9cb3905b652c494f";

            //var option = new DbContextOptionsBuilder<SqlLiteDbContext>()
            //    .UseNpgsql($"Host={host};Port={port};Database=EmbyStat;Username={user};Password={password}",
            //        x => x.MigrationsAssembly("EmbyStat.Migrations")).Options;

            var option = new DbContextOptionsBuilder<SqlLiteDbContext>()
                 .UseSqlite("Data Source=SqliteData.db",
                     x => x.MigrationsAssembly("EmbyStat.Migrations")).Options;
            return new SqlLiteDbContext(option);
        }
    }
}
