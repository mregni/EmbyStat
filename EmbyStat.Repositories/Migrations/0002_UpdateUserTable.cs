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
            Delete.Table(Constants.Tables.User);

            Create.Table(Constants.Tables.User)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_User")
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("Deleted").AsBoolean().NotNullable()
                .WithColumn("ServerId").AsGuid().NotNullable().WithDefaultValue(Guid.NewGuid())
                .WithColumn("HasPassword").AsBoolean().NotNullable()
                .WithColumn("AuthenticationProviderId").AsString().NotNullable()
                .WithColumn("HasConfiguredPassword").AsBoolean().NotNullable()
                .WithColumn("HasConfiguredEasyPassword").AsBoolean().NotNullable()
                .WithColumn("PlayDefaultAudioTrack").AsBoolean().NotNullable()
                .WithColumn("DisplayMissingEpisodes").AsBoolean().NotNullable()
                .WithColumn("DisplayCollectionsView").AsBoolean().NotNullable()
                .WithColumn("EnableLocalPassword").AsBoolean().NotNullable()
                .WithColumn("HidePlayedInLatest").AsBoolean().NotNullable()
                .WithColumn("RememberAudioSelections").AsBoolean().NotNullable()
                .WithColumn("RememberSubtitleSelections").AsBoolean().NotNullable()
                .WithColumn("EnableNextEpisodeAutoPlay").AsBoolean().NotNullable()
                .WithColumn("SubtitleMode").AsString().NotNullable()
                .WithColumn("IsAdministrator").AsBoolean().NotNullable()
                .WithColumn("MaxParentalRating").AsInt64().Nullable()
                .WithColumn("EnableUserPreferenceAccess").AsBoolean().NotNullable()
                .WithColumn("EnableRemoteControlOfOtherUsers").AsBoolean().NotNullable()
                .WithColumn("EnableSharedDeviceControl").AsBoolean().NotNullable()
                .WithColumn("EnableRemoteAccess").AsBoolean().NotNullable()
                .WithColumn("EnableLiveTvManagement").AsBoolean().NotNullable()
                .WithColumn("EnableLiveTvAccess").AsBoolean().NotNullable()
                .WithColumn("EnableMediaPlayback").AsBoolean().NotNullable()
                .WithColumn("EnableAudioPlaybackTranscoding").AsBoolean().NotNullable()
                .WithColumn("EnableVideoPlaybackTranscoding").AsBoolean().NotNullable()
                .WithColumn("EnablePlaybackRemuxing").AsBoolean().NotNullable()
                .WithColumn("EnableContentDeletion").AsBoolean().NotNullable()
                .WithColumn("EnableContentDownloading").AsBoolean().NotNullable()
                .WithColumn("EnableSubtitleDownloading").AsBoolean().NotNullable()
                .WithColumn("EnableSubtitleManagement").AsBoolean().NotNullable()
                .WithColumn("EnableSyncTranscoding").AsBoolean().NotNullable()
                .WithColumn("EnableMediaConversion").AsBoolean().NotNullable()
                .WithColumn("EnableAllDevices").AsBoolean().NotNullable()
                .WithColumn("EnableAllChannels").AsBoolean().NotNullable()
                .WithColumn("EnableAllFolders").AsBoolean().NotNullable()
                .WithColumn("EnablePublicSharing").AsBoolean().NotNullable()
                .WithColumn("DisablePremiumFeatures").AsBoolean().NotNullable()
                .WithColumn("InvalidLoginAttemptCount").AsInt64().NotNullable()
                .WithColumn("RemoteClientBitRateLimit").AsInt64().NotNullable()
                .WithColumn("BlockUnratedItems").AsString().NotNullable()
                .WithColumn("EnabledDevices").AsString().NotNullable()
                .WithColumn("EnabledChannels").AsString().NotNullable()
                .WithColumn("EnabledFolders").AsString().NotNullable()
                .WithColumn("ExcludedSubFolders").AsString().NotNullable()
                .WithColumn("BlockedTags").AsString().NotNullable()
                .WithColumn("IsHidden").AsBoolean().NotNullable()
                .WithColumn("IsDisabled").AsBoolean().NotNullable()
                .WithColumn("LastLoginDate").AsString().Nullable()
                .WithColumn("LastActivityDate").AsString().Nullable();

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
            Delete.Table(Constants.Tables.User);

            Create.Table(Constants.Tables.User)
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey("PK_User")
                .WithColumn("LastActivityDate").AsDateTime().Nullable()
                .WithColumn("LastLoginDate").AsDateTime().Nullable()
                .WithColumn("Name").AsString(100).NotNullable()
                .WithColumn("IsAdmin").AsBoolean().NotNullable()
                .WithColumn("IsHidden").AsBoolean().NotNullable()
                .WithColumn("IsDisabled").AsBoolean().NotNullable();
        }
    }
}
