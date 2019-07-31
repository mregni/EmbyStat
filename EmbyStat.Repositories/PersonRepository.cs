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

        public Person GetPersonByName(string id)
        {
            return _personCollection.FindOne(x => x.Id == id);
        }
    }
}
