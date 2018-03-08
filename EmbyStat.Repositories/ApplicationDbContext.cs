using EmbyStat.Repositories.Config;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories
{
    public class ApplicationDbContext : DbContext
    {
	    public DbSet<Configuration> Configuration { get; set; }

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

	    protected override void OnModelCreating(ModelBuilder builder)
	    {
		    base.OnModelCreating(builder);

		    builder.Entity<Configuration>().Property(s => s.Id).IsRequired();
		    builder.Entity<Configuration>().Property(s => s.Language).IsRequired();
		}
	}
}
