using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common;
using FluentMigrator;

namespace EmbyStat.Repositories.Migrations
{
    [Migration(9)]
    public class AlterEpisodeTable : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table(Constants.Tables.Media)
                .AddColumn("ShowName").AsString().Nullable();
        }
    }
}
