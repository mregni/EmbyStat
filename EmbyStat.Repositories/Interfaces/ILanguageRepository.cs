using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface ILanguageRepository
    {
        Task<List<Language>> GetLanguages();
    }
}
