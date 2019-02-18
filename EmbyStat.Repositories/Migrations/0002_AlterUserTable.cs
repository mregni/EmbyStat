using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using EmbyStat.Common;
using FluentMigrator;

namespace EmbyStat.Repositories.Migrations
{
    [Migration(2)]
    public class AlterUserTable : Migration
    {
        public override void Up()
        {
            Delete.Table(Constants.Tables.User);

            Create.Table(Constants.Tables.User)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey("PK_User")
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("ServerId").AsGuid().NotNullable().WithDefaultValue(Guid.NewGuid())
                .WithColumn("HasPassword").AsBoolean().NotNullable()
                .WithColumn("HasConfiguredPassword").AsBoolean().NotNullable()
                .WithColumn("HasConfiguredEasyPassword").AsBoolean().NotNullable()
                .WithColumn("LastLoginDate").AsString().Nullable()
                .WithColumn("LastActivityDate").AsString().Nullable()
                .WithColumn("PlayDefaultAudioTrack").AsBoolean().NotNullable()
                .WithColumn("SubtitleMode").AsString().NotNullable()
                .WithColumn("EnableLocalPassword").AsBoolean().Nullable()
                .WithColumn("IsAdministrator").AsBoolean().NotNullable()
                .WithColumn("IsHidden").AsBoolean().NotNullable()
                .WithColumn("IsDisabled").AsBoolean().NotNullable()
                .WithColumn("MaxParentalRating").AsInt64().Nullable()
                .WithColumn("BlockedTags").AsString().NotNullable()
                .WithColumn("BlockUnratedItems").AsString().NotNullable()
                .WithColumn("InvalidLoginAttemptCount").AsInt64().NotNullable()
                .WithColumn("RemoteClientBitRateLimit").AsInt64().NotNullable()
                .WithColumn("DisablePremiumFeatures").AsBoolean().NotNullable()
                .WithColumn("Deleted").AsBoolean().NotNullable()
                .WithColumn("PrimaryImageTag").AsString().Nullable();

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
