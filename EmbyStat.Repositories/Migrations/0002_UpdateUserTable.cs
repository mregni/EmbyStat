using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using EmbyStat.Common;
using FluentMigrator;

namespace EmbyStat.Repositories.Migrations
{
    [Migration(2)]
    public class UpdateUserTable : Migration
    {
        public override void Up()
        {
            Delete.Column("IsAdmin")
                .FromTable(Constants.Tables.User);

            Alter.Table(Constants.Tables.User)
                .AddColumn("Deleted").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("ServerId").AsGuid().NotNullable().WithDefaultValue(Guid.NewGuid())
                .AddColumn("HasPassword").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("HasConfiguredPassword").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("HasConfiguredEasyPassword").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("PlayDefaultAudioTrack").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("DisplayMissingEpisodes").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("DisplayCollectionsView").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnableLocalPassword").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("HidePlayedInLatest").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("RememberAudioSelections").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("RememberSubtitleSelections").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnableNextEpisodeAutoPlay").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("SubtitleMode").AsString().NotNullable().WithDefaultValue(string.Empty)
                .AddColumn("IsAdministrator").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("MaxParentalRating").AsInt64().NotNullable().WithDefaultValue(0)
                .AddColumn("EnableUserPreferenceAccess").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnableRemoteControlOfOtherUsers").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnableSharedDeviceControl").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnableRemoteAccess").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnableLiveTvManagement").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnableLiveTvAccess").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnableMediaPlayback").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnableAudioPlaybackTranscoding").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnableVideoPlaybackTranscoding").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnablePlaybackRemuxing").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnableContentDeletion").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnableContentDownloading").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnableSubtitleDownloading").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnableSubtitleManagement").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnableSyncTranscoding").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnableMediaConversion").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnableAllDevices").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnableAllChannels").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnableAllFolders").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("EnablePublicSharing").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("DisablePremiumFeatures").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("InvalidLoginAttemptCount").AsInt64().NotNullable().WithDefaultValue(0)
                .AddColumn("RemoteClientBitRateLimit").AsInt64().NotNullable().WithDefaultValue(0)
                .AlterColumn("IsHidden").AsBoolean().NotNullable().WithDefaultValue(false)
                .AlterColumn("IsDisabled").AsBoolean().NotNullable().WithDefaultValue(false)
                .AlterColumn("LastLoginDate").AsString().Nullable()
                .AlterColumn("LastActivityDate").AsString().Nullable()
                .AlterColumn("Id").AsString();

            Create.Table(Constants.Tables.UserAccessSchedules)
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey("PK_UserAccessSchedules")
                .WithColumn("DayOfWeek").AsString().NotNullable()
                .WithColumn("StartHour").AsInt64().NotNullable()
                .WithColumn("EndHour").AsInt64().NotNullable()
                .WithColumn("UserId").AsString().NotNullable()
                    .ForeignKey("FK_UserAccessSchedules_User_Id", Constants.Tables.User, "Id").OnDelete(Rule.Cascade);

            Create.Index("IX_UserAccessSchedules_UserId").OnTable(Constants.Tables.UserAccessSchedules).OnColumn("UserId");
        }

        public override void Down()
        {
            Delete.Table(Constants.Tables.UserAccessSchedules);

            Delete
                .Column("ServerId")
                .Column("HasPassword")
                .Column("HasConfiguredPassword")
                .Column("HasConfiguredEasyPassword")
                .Column("PlayDefaultAudioTrack")
                .Column("DisplayMissingEpisodes")
                .Column("DisplayCollectionsView")
                .Column("EnableLocalPassword")
                .Column("HidePlayedInLatest")
                .Column("RememberAudioSelections")
                .Column("RememberSubtitleSelections")
                .Column("EnableNextEpisodeAutoPlay")
                .Column("SubtitleMode")
                .Column("IsAdministrator")
                .Column("MaxParentalRating")
                .Column("EnableUserPreferenceAccess")
                .Column("EnableRemoteControlOfOtherUsers")
                .Column("EnableSharedDeviceControl")
                .Column("EnableRemoteAccess")
                .Column("EnableLiveTvManagement")
                .Column("EnableLiveTvAccess")
                .Column("EnableMediaPlayback")
                .Column("EnableAudioPlaybackTranscoding")
                .Column("EnableVideoPlaybackTranscoding")
                .Column("EnablePlaybackRemuxing")
                .Column("EnableContentDeletion")
                .Column("EnableContentDownloading")
                .Column("EnableSubtitleManagement")
                .Column("EnableSyncTranscoding")
                .Column("EnableMediaConversion")
                .Column("EnableAllDevices")
                .Column("EnableAllChannels")
                .Column("EnableAllFolders")
                .Column("EnablePublicSharing")
                .Column("DisablePremiumFeatures")
                .Column("InvalidLoginAttemptCount")
                .Column("RemoteClientBitRateLimit")
                .FromTable(Constants.Tables.User);

            Alter.Table(Constants.Tables.User)
                .AddColumn("IsAdmin").AsBoolean()
                .AlterColumn("IsHidden").AsBoolean().WithDefaultValue(false)
                .AlterColumn("IsDisabled").AsBoolean().WithDefaultValue(false)
                .AlterColumn("LastActivityDate").AsDateTime().Nullable()
                .AlterColumn("LastLoginDate").AsDateTime().Nullable()
                .AlterColumn("Id").AsGuid();
        }
    }
}
