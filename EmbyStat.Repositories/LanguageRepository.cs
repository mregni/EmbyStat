using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class LanguageRepository : ILanguageRepository
    {
        public IEnumerable<Language> GetLanguages()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Languages.OrderBy(x => x.Name).ToList();
            }
        }
    }
}
