using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces;

public interface IGenreRepository
{
    Task UpsertRange(IEnumerable<Genre> genres);
    Task<Genre[]> GetAll();
    Task DeleteAll();
}