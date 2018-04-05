using Microsoft.EntityFrameworkCore.Migrations;

namespace EmbyStat.Repositories.Migrations
{
    public partial class AddedConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessToken = table.Column<string>(nullable: true),
                    EmbyServerAddress = table.Column<string>(nullable: true),
                    EmbyUserName = table.Column<string>(nullable: true),
                    Language = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    WizardFinished = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuration");
        }
    }
}
