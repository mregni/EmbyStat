using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Migrator.Models;
using LiteDB;

namespace EmbyStat.Migrator.Migrations
{
    [Migration(4)]
    public class SetLocationTypeOnEpisodes : Migration
    {
        public override void Up()
        {
            try
            {
                var dbPath = Path.Combine(AppSettings.Dirs.Config, AppSettings.DatabaseFile).GetLocalPath();
                var context = new LiteDatabase(dbPath);

                var episodeCollection = context.GetCollection("Episode");
                foreach (var episode in episodeCollection.FindAll())
                {
                    episode["LocationType"] = (int) LocationType.Disk;
                    episodeCollection.Update(episode);
                }
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException("Can find or create LiteDb database.", ex);
            }
        }
    }
}
