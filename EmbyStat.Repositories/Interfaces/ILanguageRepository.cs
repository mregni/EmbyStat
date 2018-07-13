using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models;

namespace EmbyStat.Repositories.Interfaces
{
    public interface ILanguageRepository
    {
        IEnumerable<Language> GetLanguages();
    }
}
