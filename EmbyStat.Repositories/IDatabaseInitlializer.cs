using System.Threading.Tasks;

namespace EmbyStat.Repositories
{
	public interface IDatabaseInitializer
	{
		Task SeedAsync();
	}
}
