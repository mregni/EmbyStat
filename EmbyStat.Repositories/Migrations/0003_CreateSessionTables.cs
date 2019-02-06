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
                .WithColumn("AppIconUrl").AsString();

            Create.Table(Constants.Tables.Plays)
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey("PK_Plays")
                .WithColumn("Type").AsString().NotNullable()
                .WithColumn("MediaId").AsString().NotNullable()
                .WithColumn("SessionId").AsString()
                    .ForeignKey("FK_Plays_Sessions_SessionId", Constants.Tables.Sessions, "Id").OnDelete(Rule.Cascade)
                .WithColumn("IsFinished").AsBoolean().NotNullable().WithDefaultValue(false);

            Create.Table(Constants.Tables.PlayStates)
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey("PK_PlayStates")
                .WithColumn("PositionTicks").AsInt64().Nullable()
                .WithColumn("CanSeek").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("IsPaused").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("IsMuted").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("VolumeLevel").AsInt32().Nullable()
                .WithColumn("AudioStreamIndex").AsInt32().Nullable()
                .WithColumn("SubtitleStreamIndex").AsInt32().Nullable()
                .WithColumn("MediaSourceId").AsString().NotNullable()
                .WithColumn("PlayMethod").AsInt16().Nullable()
                .WithColumn("RepeatMode").AsInt16().NotNullable()
                .WithColumn("PlayId").AsString().NotNullable()
                    .ForeignKey("FK_PlayStates_Plays_PlayId", Constants.Tables.Plays, "Id").OnDelete(Rule.Cascade);

            Create.Table(Constants.Tables.TranscodingInfos)
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey("PK_TranscodingInfos")
                .WithColumn("AudioCodec").AsString().NotNullable()
                .WithColumn("VideoCodec").AsString().NotNullable()
                .WithColumn("Container").AsString().NotNullable()
                .WithColumn("IsVideoDirect").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("IsAudioDirect").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("Bitrate").AsInt32().Nullable()
                .WithColumn("Framerate").AsFloat().Nullable()
                .WithColumn("CompletionPercentage").AsDouble().Nullable()
                .WithColumn("Width").AsInt32().Nullable()
                .WithColumn("Height").AsInt32().Nullable()
                .WithColumn("AudioChannels").AsInt32().Nullable()
                .WithColumn("TranscodeReasons").AsString()
                .WithColumn("PlayId").AsString().NotNullable()
                    .ForeignKey("FK_TranscodeingInfos_Plays_PlayId", Constants.Tables.Plays, "Id").OnDelete(Rule.Cascade);

            Create.Index("IX_TranscodingInfos_PlayId").OnTable(Constants.Tables.TranscodingInfos).OnColumn("PlayId");
            Create.Index("IX_PlayStates_PlayId").OnTable(Constants.Tables.PlayStates).OnColumn("PlayId");
            Create.Index("IX_Plays_SessionId").OnTable(Constants.Tables.Plays).OnColumn("SessionId");
        }
    }
}