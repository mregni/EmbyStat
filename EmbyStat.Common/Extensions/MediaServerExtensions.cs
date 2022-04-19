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
LEFT JOIN {Constants.Tables.MediaServerUserView} AS uc ON (u.Id = uc.UserId)
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
}