using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly DbContext _context;
        public LanguageRepository(DbContext context)
        {
            _context = context;
        }

        public Task<List<Language>> GetLanguages()
        {
            return _context.Languages.ToListAsync();
        }
    }
}
