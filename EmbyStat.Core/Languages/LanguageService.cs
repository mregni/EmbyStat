using EmbyStat.Common.Models.Entities;
using EmbyStat.Core.Languages.Interfaces;

namespace EmbyStat.Core.Languages;

public class LanguageService : ILanguageService
{
    private readonly ILanguageRepository _languageRepository;

    public LanguageService(ILanguageRepository languageRepository)
    {
        _languageRepository = languageRepository;
    }

    public Task<List<Language>> GetLanguages()
    {
        return _languageRepository.GetLanguages();
    }
}