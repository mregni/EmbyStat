using EmbyStat.Migrator.Models;

namespace EmbyStat.Migrator.Migrations
{
    [Migration(6)]
    public class EditUserSettings : Migration
    {
        public override void Up()
        {
            if (UserSettings.Emby != null)
            {
                UserSettings.MediaServer = UserSettings.Emby;
                UserSettings.Emby = null;
            }
        }
    }
}
