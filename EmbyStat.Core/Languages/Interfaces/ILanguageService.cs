using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Core.Languages.Interfaces;

public interface ILanguageService
{
    Task<List<Language>> GetLanguages();
}