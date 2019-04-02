using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _context;

        public PersonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddRangeIfMissing(IEnumerable<Person> people)
        {
            var newPeople = people.Where(x => _context.People.AsNoTracking().All(y => y.Name != x.Name)).ToList();

            _context.People.AddRange(newPeople);
            _context.SaveChanges();
        }

        public List<string> GetIds()
        {
            return _context.People.Select(x => x.Id).ToList();
        }

        public Person GetPersonById(string id)
        {
            return _context.People.SingleOrDefault(x => x.Id == id);
        }

        public void AddOrUpdatePerson(Person person)
        {
            var localPerson = _context.People.AsNoTracking().SingleOrDefault(x => x.Id == person.Id);
            if (localPerson == null)
            {
                _context.People.Add(person);
                _context.SaveChanges();
            }
            else
            {
                _context.SaveChanges();
            }
        }

        public async Task CleanupPersons()
        {
            var peopleToRemove = _context.People
                .Include(x => x.ExtraPersons)
                .Where(x => x.ExtraPersons.Count == 0);

            _context.People.RemoveRange(peopleToRemove);
            await _context.SaveChangesAsync();
        }
    }
}
