using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public List<Genre> GetAll()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Genres.ToList();
            }
        }

        public List<Guid> GetIds()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Genres.Select(x => x.Id).ToList();
            }
        }

        public List<Genre> GetListByIds(List<Guid> ids)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Genres.Where(x => ids.Any(y => y == x.Id)).ToList();
            }
        }

        public async Task CleanupGenres()
        {
            using (var context = new ApplicationDbContext())
            {
                var genresToRemove = context.Genres
                    .Include(x => x.MediaGenres)
                    .Where(x => x.MediaGenres.Count == 0);

                context.Genres.RemoveRange(genresToRemove);
                await context.SaveChangesAsync();
            }
        }
    }
}
