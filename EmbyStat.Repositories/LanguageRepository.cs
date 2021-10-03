﻿using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class LanguageRepository : BaseRepository, ILanguageRepository
    {
        public LanguageRepository(IDbContext context) : base(context)
        {
        }

        public List<Language> GetLanguages()
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Language>();
            return collection.FindAll().ToList();
        }
    }
}
