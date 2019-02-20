using FluentMigrator;

namespace EmbyStat.Repositories.Migrations
{
    [Migration(7)]
    public class DeleteDrivesTable : Migration
    {
        public override void Up()
        {
            Delete.Table("Drives");
        }

        public override void Down()
        {
            Create.Table("Drives")
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_Drives")
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("Path").AsString().NotNullable()
                .WithColumn("Type").AsString().NotNullable();
        }
    }
}
