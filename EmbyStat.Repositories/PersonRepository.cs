using System;
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
        public void AddRangeIfMissing(IEnumerable<Person> people)
        {
            using (var context = new ApplicationDbContext())
            {
                var newPeople = people.Where(x => context.People.AsNoTracking().All(y => y.Name != x.Name)).ToList();

                context.People.AddRange(newPeople);
                context.SaveChanges();
            }
        }

        public List<Guid> GetIds()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.People.Select(x => x.Id).ToList();
            }
        }

        public Person GetPersonById(Guid id)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.People.SingleOrDefault(x => x.Id == id);
            }
        }

        public void AddOrUpdatePerson(Person person)
        {
            using (var context = new ApplicationDbContext())
            {
                var localPerson = context.People.AsNoTracking().SingleOrDefault(x => x.Id == person.Id);
                if (localPerson == null)
                {
                    context.People.Add(person);
                    context.SaveChanges();
                }
                else
                {
                    context.Entry(person).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }

        public async Task CleanupPersons()
        {
            using (var context = new ApplicationDbContext())
            {
                var peopleToRemove = context.People
                    .Include(x => x.ExtraPersons)
                    .Where(x => x.ExtraPersons.Count == 0);

                context.People.RemoveRange(peopleToRemove);
                await context.SaveChangesAsync();
            }
        }
    }
}
