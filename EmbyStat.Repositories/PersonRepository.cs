using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using LiteDB;

namespace EmbyStat.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly LiteCollection<Person> _personCollection;

        public PersonRepository(IDbContext context)
        {
            _personCollection = context.GetContext().GetCollection<Person>();
        }

        public void Insert(Person person)
        {
            _personCollection.Insert(person);
        }

        public Person GetPersonByName(string name)
        {
            return _personCollection.FindOne(x => x.Name == name);
        }
    }
}
