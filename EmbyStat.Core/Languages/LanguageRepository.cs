using EmbyStat.Common.Models.Entities;
using EmbyStat.Core.DataStore;
using EmbyStat.Core.Languages.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Core.Languages;

public class LanguageRepository : ILanguageRepository
{
    private readonly EsDbContext _context;
    public LanguageRepository(EsDbContext context)
    {
        _context = context;
    }

    public Task<List<Language>> GetLanguages()
    {
        return _context.Languages.AsNoTracking().ToListAsync();
    }
}