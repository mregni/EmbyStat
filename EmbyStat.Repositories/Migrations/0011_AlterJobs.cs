using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common;
using FluentMigrator;

namespace EmbyStat.Repositories.Migrations
{
    [Migration(11)]
    public class AlterJobs : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table(Constants.Tables.Jobs)
                .AddColumn("LogName").AsString().Nullable();
        }
    }
}
