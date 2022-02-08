using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class PersonRepository : BaseRepository, IPersonRepository
    {
        public PersonRepository(IDbContext context) : base(context)
        {

        }

        public void Upsert(Person person)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Person>();
            collection.Upsert(person);
        }

        public Task UpsertRange(IEnumerable<Common.SqLite.SqlPerson> people)
        {
            throw new System.NotImplementedException();
        }

        public Person GetPersonByName(string name)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Person>();
            return collection.FindOne(x => x.Name == name);
        }
    }
}
