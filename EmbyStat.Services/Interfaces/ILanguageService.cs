using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Services.Interfaces
{
    public interface ILanguageService
    {
        IEnumerable<Language> GetLanguages();
    }
}
