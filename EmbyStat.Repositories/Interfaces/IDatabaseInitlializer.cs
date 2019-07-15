namespace EmbyStat.Repositories.Interfaces
{
	public interface IDatabaseInitializer
    {
        void CreateIndexes();
        void SeedAsync();
	}
}
