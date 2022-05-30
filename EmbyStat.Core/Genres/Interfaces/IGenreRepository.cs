using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Core.Genres.Interfaces;

public interface IGenreRepository
{
    Task UpsertRange(IEnumerable<Genre> genres);
    Task<Genre[]> GetAll();
    Task DeleteAll();
}