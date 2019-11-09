using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using LiteDB;

namespace EmbyStat.Repositories
{
    public class PersonRepository : BaseRepository, IPersonRepository
    {
        public PersonRepository(IDbContext context) : base(context)
        {

        }

        public void Insert(Person person)
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Person>();
                    collection.Insert(person);
                }
            });
        }

        public Person GetPersonByName(string name)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Person>();
                    return collection.FindOne(x => x.Name == name);
                }
            });
        }
    }
}
