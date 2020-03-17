using System;
using System.IO;
using System.Linq;
using EmbyStat.Common.Extensions;
using EmbyStat.Migrator.Models;
using LiteDB;
using MediaBrowser.Model.Entities;

namespace EmbyStat.Migrator.Migrations
{
    [Migration(3)]
    public class PersonTypeStringToEnum : Migration
    {
        public override void Up()
        {
            try
            {
                var dbPath = Path.Combine(AppSettings.Dirs.Data, AppSettings.DatabaseFile);
                using (var context = new LiteDatabase($"FileName={dbPath};"))
                {
                    var movieCollection = context.GetCollection("Movie");
                    MigrateItems(movieCollection);

                    var showCollection = context.GetCollection("Show");
                    MigrateItems(showCollection);   
                }
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException("Can't find or create LiteDb database.", ex);
            }
        }

        private void MigrateItems(ILiteCollection<BsonDocument> collection)
        {
            var entities = collection.FindAll().ToList();

            foreach (var entity in entities)
            {
                foreach (var person in entity["People"].AsArray)
                {
                    if (person.AsDocument["Type"] == "Actor")
                    {
                        person.AsDocument["Type"] = ((int)PersonType.Actor).ToString();
                    }
                    else if (person.AsDocument["Type"] == "Director")
                    {
                        person.AsDocument["Type"] = ((int)PersonType.Director).ToString();
                    }
                    else if (person.AsDocument["Type"] == "Writer")
                    {
                        person.AsDocument["Type"] = ((int)PersonType.Writer).ToString();
                    }
                }

                collection.Update(entity);
            }
        }
    }
}
