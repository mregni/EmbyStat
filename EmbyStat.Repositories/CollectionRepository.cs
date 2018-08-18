using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories
{
    public class CollectionRepository : ICollectionRepository
    {
        public IEnumerable<Collection> GetCollectionByTypes(IEnumerable<CollectionType> types)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Collections.Where(x => types.Any(y => y == x.Type)).ToList();
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
