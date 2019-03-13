using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly ApplicationDbContext _context;

        public LanguageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Language> GetLanguages()
        {
            return _context.Languages.OrderBy(x => x.Name).ToList();
        }
    }
}
