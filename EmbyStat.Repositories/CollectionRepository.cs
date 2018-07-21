using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public void AddOrUpdateRange(IEnumerable<Collection> collections)
        {
            using (var context = new ApplicationDbContext())
            {
                foreach (var collection in collections)
                {
                    var dbCollection = context.Collections.AsNoTracking().FirstOrDefault(x => x.Id == collection.Id);

                    if (dbCollection == null)
                    {
                        context.Add(collection);
                    }
                    else
                    {
                        context.Entry(collection).State = EntityState.Modified;
                    }
                }
                context.SaveChanges();
            }
        }
    }
}
