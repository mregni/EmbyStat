using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using LiteDB;

namespace EmbyStat.Repositories
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly LiteCollection<Language> _languageCollection;

        public LanguageRepository(IDbContext context)
        {
            _languageCollection = context.GetContext().GetCollection<Language>();
        }

        public IEnumerable<Language> GetLanguages()
        {
            return _languageCollection.FindAll();
        }
    }
}
