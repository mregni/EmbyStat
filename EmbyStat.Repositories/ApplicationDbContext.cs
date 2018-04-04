using EmbyStat.Common.Tasks;
using EmbyStat.Repositories.Config;
using EmbyStat.Repositories.EmbyDrive;
using EmbyStat.Repositories.EmbyServerInfo;
using MediaBrowser.Model.Plugins;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories
{
    public class ApplicationDbContext : DbContext
    {
	    public DbSet<Configuration> Configuration { get; set; }
		public DbSet<PluginInfo> Plugins { get; set; }
		public DbSet<ServerInfo> ServerInfo { get; set; }
		public DbSet<Drives> Drives { get; set; }
        public DbSet<TaskResult> TaskResults { get; set; }
        public DbSet<TaskTriggerInfo> TaskTriggerInfos { get; set; }

        public ApplicationDbContext() : base()
	    {

	    }

	    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	    {

	    }

	    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	    {
		    base.OnConfiguring(optionsBuilder);
		    optionsBuilder.UseSqlite("Data Source=data.db");
	    }

	    protected override void OnModelCreating(ModelBuilder modelBuilder)
	    {
		    base.OnModelCreating(modelBuilder);

		    modelBuilder.Entity<Configuration>().Property(s => s.Id).IsRequired();
		    modelBuilder.Entity<Configuration>().Property(s => s.Language).IsRequired();

		    modelBuilder.Entity<PluginInfo>().Property(s => s.Id).IsRequired();

		    modelBuilder.Entity<ServerInfo>().Property(s => s.Id).IsRequired();

	        modelBuilder.Entity<TaskTriggerInfo>().Property(t => t.Id).IsRequired();

	    }
	}
}
