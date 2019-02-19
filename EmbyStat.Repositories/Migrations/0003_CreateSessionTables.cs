using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using EmbyStat.Common;
using FluentMigrator;

namespace EmbyStat.Repositories.Migrations
{
    [Migration(3)]
    public class CreateSessionTables : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table(Constants.Tables.Sessions)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_Sessions")
                .WithColumn("ServerId").AsString().NotNullable()
                .WithColumn("UserId").AsString().NotNullable()
                .WithColumn("ApplicationVersion").AsString().NotNullable()
                .WithColumn("Client").AsString().NotNullable()
                .WithColumn("DeviceId").AsString().NotNullable()
                .WithColumn("AppIconUrl").AsString().Nullable();

            Create.Table(Constants.Tables.Plays)
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey("PK_Plays")
                .WithColumn("Type").AsString().NotNullable()
                .WithColumn("ParentId").AsString().NotNullable()
                .WithColumn("MediaId").AsString().NotNullable()
                .WithColumn("SessionId").AsString().NotNullable()
                .ForeignKey("FK_Plays_Sessions_SessionId", Constants.Tables.Sessions, "Id").OnDelete(Rule.Cascade)
                .WithColumn("SubtitleCodec").AsString().Nullable()
                .WithColumn("SubtitleLanguage").AsString().Nullable()
                .WithColumn("SubtitleDisplayTitle").AsString().Nullable()
                .WithColumn("AudioCodec").AsString().Nullable()
                .WithColumn("AudioLanguage").AsString().Nullable()
                .WithColumn("AudioChannelLayout").AsString().Nullable()
                .WithColumn("VideoCodec").AsString().Nullable()
                .WithColumn("VideoHeight").AsInt32().Nullable()
                .WithColumn("VideoWidth").AsInt32().Nullable()
                .WithColumn("VideoAverageFrameRate").AsDouble().Nullable()
                .WithColumn("VideoRealFrameRate").AsDouble().Nullable()
                .WithColumn("VideoAspectRatio").AsString().Nullable();

            Create.Table(Constants.Tables.PlayStates)
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey("PK_PlayStates")
                .WithColumn("PositionTicks").AsInt64()
                .WithColumn("VolumeLevel").AsInt32()
                .WithColumn("MediaSourceId").AsString().NotNullable()
                .WithColumn("IsPaused").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("PlayMethod").AsInt16().Nullable()
                .WithColumn("RepeatMode").AsInt16().NotNullable()
                .WithColumn("Framerate").AsFloat().Nullable()
                .WithColumn("IsTranscoding").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("AudioCodec").AsString().Nullable()
                .WithColumn("VideoCodec").AsString().Nullable()
                .WithColumn("TranscodeReasons").AsString().Nullable()
                .WithColumn("TimeLogged").AsDateTime().NotNullable()
                .WithColumn("PlayId").AsString().NotNullable()
                    .ForeignKey("FK_PlayStates_Plays_PlayId", Constants.Tables.Plays, "Id").OnDelete(Rule.Cascade);

            Create.Index("IX_PlayStates_PlayId").OnTable(Constants.Tables.PlayStates).OnColumn("PlayId");
            Create.Index("IX_Plays_SessionId").OnTable(Constants.Tables.Plays).OnColumn("SessionId");
        }
    }
}