using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Core.Languages.Interfaces;

public interface ILanguageRepository
{
    Task<List<Language>> GetLanguages();
}