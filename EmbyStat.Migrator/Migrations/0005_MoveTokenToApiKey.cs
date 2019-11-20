using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Migrator.Models;

namespace EmbyStat.Migrator.Migrations
{
    [Migration(5)]
    public class MoveTokenToApiKey : Migration
    {
        public override void Up()
        {
            UserSettings.Emby.ApiKey = UserSettings.Emby.AccessToken;
        }
    }
}
