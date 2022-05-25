using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Core.DataStore;
using EmbyStat.Core.Filters.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Core.Filters;

public class FilterRepository : IFilterRepository
{
    private readonly EsDbContext _context;

    public FilterRepository(EsDbContext context)
    {
        _context = context;
    }

    public Task<FilterValues> Get(LibraryType type, string field)
    {
        return _context.Filters.FirstOrDefaultAsync(x => x.Field == field && x.Type == type);
    }

    public async Task Insert(FilterValues values)
    {
        _context.Filters.Add(values);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAll(LibraryType type)
    {
        _context.RemoveRange(_context.Filters.Where(x => x.Type == type));
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAll()
    {
        _context.RemoveRange(_context.Filters);
        await _context.SaveChangesAsync();
    }
}