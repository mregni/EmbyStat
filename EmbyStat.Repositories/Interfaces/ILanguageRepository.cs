using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface ILanguageRepository
    {
        List<Language> GetLanguages();
    }
}
