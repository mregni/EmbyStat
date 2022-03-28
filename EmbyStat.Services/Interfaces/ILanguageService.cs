using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Services.Interfaces
{
    public interface ILanguageService
    {
        Task<List<Language>> GetLanguages();
    }
}
