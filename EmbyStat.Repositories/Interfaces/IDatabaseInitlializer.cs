using System.Threading.Tasks;

namespace EmbyStat.Repositories.Interfaces
{
	public interface IDatabaseInitializer
	{
		Task SeedAsync();
	}
}
