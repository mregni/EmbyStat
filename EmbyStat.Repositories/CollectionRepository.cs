using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class CollectionRepository : ICollectionRepository
    {
        public IEnumerable<Collection> GetCollectionByType(CollectionType type)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Collections.Where(x => x.Type == type).ToList();
            }
        }

        public void AddCollectionRange(IEnumerable<Collection> collections)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Collections.AddRange(collections);
                context.SaveChanges();
            }
        }

        public void RemoveCollectionByType(CollectionType type)
        {
            using (var context = new ApplicationDbContext())
            {
                var needRemoval = context.Collections.Where(x => x.Type == type);
                context.Collections.RemoveRange(needRemoval);
                context.SaveChanges();
            }
        }
    }
}
