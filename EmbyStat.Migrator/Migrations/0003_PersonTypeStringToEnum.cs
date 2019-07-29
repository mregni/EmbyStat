using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EmbyStat.Common.Models.Entities;
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
                var dbPath = Path.Combine(Directory.GetCurrentDirectory(), AppSettings.Dirs.Database, AppSettings.DatabaseFile);
                var context = new LiteDatabase(dbPath);

                var movieCollection = context.GetCollection("Movie");
                MigrateItems(movieCollection);

                var showCollection = context.GetCollection("Show");
                MigrateItems(showCollection);
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException("Can find or create LiteDb database.", ex);
            }
        }

        private void MigrateItems(LiteCollection<BsonDocument> collection)
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
