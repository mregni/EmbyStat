using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common;
using FluentMigrator;

namespace EmbyStat.Repositories.Migrations
{
    [Migration(6)]
    public class AlterMediaTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Index("IX_Media_ParentId").OnTable(Constants.Tables.Media).OnColumn("ParentId");
        }
    }
}
