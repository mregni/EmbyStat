using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface ILanguageRepository
    {
        IEnumerable<Language> GetLanguages();
    }
}
