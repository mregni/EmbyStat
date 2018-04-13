using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        public void AddRangeIfMissing(IEnumerable<Genre> genres)
        {
            using (var context = new ApplicationDbContext())
            {
                var newGenres = genres.Where(x => context.Genres.All(y => y.Name != x.Name)).ToList();

                context.Genres.AddRange(newGenres);
                context.SaveChanges();
            }
        }

        public List<string> GetIds()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Genres.Select(x => x.Id).ToList();
            }
        }
    }
}
