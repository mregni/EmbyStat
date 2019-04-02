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
        private readonly ApplicationDbContext _context;

        public GenreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddRangeIfMissing(IEnumerable<Genre> genres)
        {
            var newGenres = genres.Where(x => _context.Genres.All(y => y.Name != x.Name)).ToList();

            _context.Genres.AddRange(newGenres);
            _context.SaveChanges();
        }

        public List<Genre> GetAll()
        {
            return _context.Genres.ToList();
        }

        public List<string> GetIds()
        {
            return _context.Genres.Select(x => x.Id).ToList();
        }

        public List<Genre> GetListByIds(List<string> ids)
        {
            return _context.Genres.Where(x => ids.Any(y => y == x.Id)).ToList();
        }

        public async Task CleanupGenres()
        {
            var genresToRemove = _context.Genres
                .Include(x => x.MediaGenres)
                .Where(x => x.MediaGenres.Count == 0);

            _context.Genres.RemoveRange(genresToRemove);
            await _context.SaveChangesAsync();
        }
    }
}
