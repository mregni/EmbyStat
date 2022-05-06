namespace EmbyStat.Common.Extensions;

public static class MediaServerExtensions
{
    public static string GenerateUserPageQuery(int skip, int take, string sortField, string sortOrder)
    {
        var query = $@"
SELECT u.Id, u.Name, u.LastActivityDate, u.IsAdministrator, u.IsHidden, u.IsDisabled
	, SUM(CASE WHEN uc.MediaType = 'Movie' THEN 1 ELSE 0 END) AS MovieViewCount
	, SUM(CASE WHEN uc.MediaType = 'Episode' THEN 1 ELSE 0 END) AS EpisodeViewCount
	, SUM(CASE WHEN uc.MediaType = 'Episode' OR uc.MediaType = 'Movie' THEN 1 ELSE 0 END) AS TotalViewCount
FROM {Constants.Tables.MediaServerUsers} AS u
LEFT JOIN {Constants.Tables.MediaServerUserViews} AS uc ON (u.Id = uc.UserId)
GROUP BY u.Id
";

        if (!string.IsNullOrWhiteSpace(sortField))
        {
            sortField = sortField.FirstCharToUpper();
            query += $"ORDER BY {sortField} {sortOrder.ToUpper()} ";
        }

        query += $"LIMIT {take} OFFSET {skip}";
        return query;
    }

    public static string UserInsertQuery => @"
INSERT INTO MediaServerUsers (
    Id, Name, ServerId, HasPassword, HasConfiguredPassword, HasConfiguredEasyPassword, PrimaryImageTag, LastActivityDate, PlayDefaultAudioTrack, SubtitleLanguagePreference,
	DisplayMissingEpisodes, SubtitleMode, IsAdministrator, IsHidden, IsHiddenRemotely, IsHiddenFromUnusedDevices, IsDisabled, EnableLiveTvAccess, EnableContentDeletion,
	EnableContentDownloading, EnableSubtitleDownloading, EnableSubtitleManagement, EnableSyncTranscoding, EnableMediaConversion, InvalidLoginAttemptCount, EnablePublicSharing,
	RemoteClientBitrateLimit, SimultaneousStreamLimit, EnableAllDevices)
VALUES (
	@Id, @Name, @ServerId, @HasPassword, @HasConfiguredPassword, @HasConfiguredEasyPassword, @PrimaryImageTag, @LastActivityDate, @PlayDefaultAudioTrack, @SubtitleLanguagePreference,
	@DisplayMissingEpisodes, @SubtitleMode, @IsAdministrator, @IsHidden, @IsHiddenRemotely, @IsHiddenFromUnusedDevices, @IsDisabled, @EnableLiveTvAccess, @EnableContentDeletion,
	@EnableContentDownloading, @EnableSubtitleDownloading, @EnableSubtitleManagement, @EnableSyncTranscoding, @EnableMediaConversion, @InvalidLoginAttemptCount, @EnablePublicSharing,
	@RemoteClientBitrateLimit, @SimultaneousStreamLimit, @EnableAllDevices)
ON CONFLICT (Id) DO
UPDATE SET Name=excluded.Name, ServerId=excluded.ServerId, HasPassword=excluded.HasPassword, HasConfiguredPassword=excluded.HasConfiguredPassword, HasConfiguredEasyPassword=excluded.HasConfiguredEasyPassword, 
	PrimaryImageTag=excluded.PrimaryImageTag, LastActivityDate=excluded.LastActivityDate, PlayDefaultAudioTrack=excluded.PlayDefaultAudioTrack, SubtitleLanguagePreference=excluded.SubtitleLanguagePreference,
	DisplayMissingEpisodes=excluded.DisplayMissingEpisodes, SubtitleMode=excluded.SubtitleMode, IsAdministrator=excluded.IsAdministrator, IsHidden=excluded.IsHidden, IsHiddenRemotely=excluded.IsHiddenRemotely, 
	IsHiddenFromUnusedDevices=excluded.IsHiddenFromUnusedDevices, IsDisabled=excluded.IsDisabled, EnableLiveTvAccess=excluded.EnableLiveTvAccess, EnableContentDeletion=excluded.EnableContentDeletion,
	EnableContentDownloading=excluded.EnableContentDownloading, EnableSubtitleDownloading=excluded.EnableSubtitleDownloading, EnableSubtitleManagement=excluded.EnableSubtitleManagement, 
	EnableSyncTranscoding=excluded.EnableSyncTranscoding, EnableMediaConversion=excluded.EnableMediaConversion, InvalidLoginAttemptCount=excluded.InvalidLoginAttemptCount, EnablePublicSharing=excluded.EnablePublicSharing,
	RemoteClientBitrateLimit=excluded.RemoteClientBitrateLimit, SimultaneousStreamLimit=excluded.SimultaneousStreamLimit, EnableAllDevices=excluded.EnableAllDevices";
}