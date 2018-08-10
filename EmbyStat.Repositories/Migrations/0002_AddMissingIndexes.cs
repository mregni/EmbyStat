using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common;
using FluentMigrator;

namespace EmbyStat.Repositories.Migrations
{
    [Migration(2)]
    public class AddMissingIndexes : AutoReversingMigration

    {
        public override void Up()
        {
            Create.Index("IX_ExtraPersons_ExtraId").OnTable(Constants.Tables.ExtraPersons).OnColumn("ExtraId");
            Create.Index("IX_SeasonEpisodes_EpisodeId").OnTable(Constants.Tables.SeasonEpisodes).OnColumn("EpisodeId");
        }
    }
}
