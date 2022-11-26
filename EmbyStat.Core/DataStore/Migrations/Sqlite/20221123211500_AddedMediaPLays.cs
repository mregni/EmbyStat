using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmbyStat.Core.DataStore.Migrations.Sqlite
{
    public partial class AddedMediaPLays : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MediaPlays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SessionId = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    MediaId = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    PlayMethod = table.Column<string>(type: "TEXT", nullable: true),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Stop = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StartPositionTicks = table.Column<long>(type: "INTEGER", nullable: false),
                    EndPositionTicks = table.Column<long>(type: "INTEGER", nullable: false),
                    WatchedPercentage = table.Column<double>(type: "REAL", nullable: false),
                    IsPaused = table.Column<bool>(type: "INTEGER", nullable: false),
                    AudioCodec = table.Column<string>(type: "TEXT", nullable: true),
                    AudioChannelLayout = table.Column<string>(type: "TEXT", nullable: true),
                    AudioSampleRate = table.Column<string>(type: "TEXT", nullable: true),
                    SubtitleCodec = table.Column<string>(type: "TEXT", nullable: true),
                    SubtitleDisplayLanguage = table.Column<string>(type: "TEXT", nullable: true),
                    SubtitleLanguage = table.Column<string>(type: "TEXT", nullable: true),
                    SubtitleProtocol = table.Column<string>(type: "TEXT", nullable: true),
                    TranscodeAverageCpuUsage = table.Column<double>(type: "REAL", nullable: true),
                    TranscodeCurrentCpuUsage = table.Column<double>(type: "REAL", nullable: true),
                    TranscodeVideoCodec = table.Column<string>(type: "TEXT", nullable: true),
                    TranscodeAudioCodec = table.Column<string>(type: "TEXT", nullable: true),
                    TranscodeSubProtocol = table.Column<string>(type: "TEXT", nullable: true),
                    TranscodeReasons = table.Column<string>(type: "TEXT", nullable: true),
                    Encoder = table.Column<string>(type: "TEXT", nullable: true),
                    EncoderIsHardware = table.Column<bool>(type: "INTEGER", nullable: true),
                    EncoderMediaType = table.Column<string>(type: "TEXT", nullable: true),
                    Decoder = table.Column<string>(type: "TEXT", nullable: true),
                    DecoderIsHardware = table.Column<bool>(type: "INTEGER", nullable: true),
                    DecoderMediaType = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaPlays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaPlays_MediaServerUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "MediaServerUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MediaPlays_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaPlays_SessionId",
                table: "MediaPlays",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaPlays_UserId",
                table: "MediaPlays",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaPlays");
        }
    }
}
