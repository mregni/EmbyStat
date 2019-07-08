using System;
using System.Collections.Generic;
using System.Linq;
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

        public void UpserRange(IEnumerable<Person> people)
        {
            _personCollection.Upsert(people);
        }

        public IEnumerable<string> GetIds()
        {
            return _personCollection.FindAll().Select(x => x.Id);
        }

        public Person GetPersonById(string id)
        {
            return _personCollection.FindOne(x => x.Id == id);
        }

        public void AddOrUpdatePerson(Person person)
        {
            _personCollection.Upsert(person);
        }

        public void CleanupPersons()
        {
            throw new NotImplementedException();
        }
    }
}
