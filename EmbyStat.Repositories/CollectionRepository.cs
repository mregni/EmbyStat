using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories
{
    public class CollectionRepository : ICollectionRepository
    {
        private readonly ApplicationDbContext _context;

        public CollectionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Collection> GetCollectionByTypes(IEnumerable<CollectionType> types)
        {
            return _context.Collections.Where(x => types.Any(y => y == x.Type)).ToList();
        }

        public void AddCollectionRange(IEnumerable<Collection> collections)
        {
            _context.Collections.AddRange(collections);
            _context.SaveChanges();
        }

        public void AddOrUpdateRange(IEnumerable<Collection> collections)
        {
            foreach (var collection in collections)
            {
                var dbCollection = _context.Collections.AsNoTracking().FirstOrDefault(x => x.Id == collection.Id);

                if (dbCollection == null)
                {
                    _context.Add(collection);
                }
                else
                {
                    _context.Entry(collection).State = EntityState.Modified;
                }
            }
            _context.SaveChanges();
        }
    }
}
