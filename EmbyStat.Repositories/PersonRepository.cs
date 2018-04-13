using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        public void AddRangeIfMissing(IEnumerable<Person> people)
        {
            using (var context = new ApplicationDbContext())
            {
                var newPeople = people.Where(x => context.People.All(y => y.Name != x.Name)).ToList();

                context.People.AddRange(newPeople);
                context.SaveChanges();
            }
        }

        public List<string> GetIds()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.People.Select(x => x.Id).ToList();
            }
        }
    }
}
