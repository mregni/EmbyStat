using EmbyStat.Migrator.Models;

namespace EmbyStat.Migrator.Migrations
{
    [Migration(2)]
    public class AddToShortMovieEnabled : Migration
    {
        public override void Up()
        {
            UserSettings.ToShortMovieEnabled = true;
        }
    }
}
