using EmbyStat.Common.Extensions;
using FluentAssertions;
using Xunit;

namespace Tests.Unit.Extensions;

public class MediServerExtensionTests
{
    [Theory]
    [InlineData(0,10,"name","asc")]
    [InlineData(1,5,"MovieViewCount","desc")]
    [InlineData(1,5,"MovieViewCount","DESV")]
    public void GenerateUserPageQuery_Should_Generate_Page(int skip, int take, string sortField, string sortOrder)
    {
        var query = MediaServerExtensions.GenerateUserPageQuery(skip, take, sortField, sortOrder);
        query.Should().Be($@"
SELECT u.Id, u.Name, u.LastActivityDate, u.IsAdministrator, u.IsHidden, u.IsDisabled
	, SUM(CASE WHEN uc.MediaType = 'Movie' THEN 1 ELSE 0 END) AS MovieViewCount
	, SUM(CASE WHEN uc.MediaType = 'Episode' THEN 1 ELSE 0 END) AS EpisodeViewCount
	, SUM(CASE WHEN uc.MediaType = 'Episode' OR uc.MediaType = 'Movie' THEN 1 ELSE 0 END) AS TotalViewCount
FROM MediaServerUsers AS u
LEFT JOIN MediaServerUserView AS uc ON (u.Id = uc.UserId)
GROUP BY u.Id
ORDER BY {sortField.FirstCharToUpper()} {sortOrder.ToUpper()} LIMIT {take} OFFSET {skip}");
    }
    
    [Theory]
    [InlineData(0,10, "")]
    [InlineData(1,5, null)]
    public void GenerateUserPageQuery_Should_Generate_Page_Without_Sorting(int skip, int take, string sortField)
    {
        var query = MediaServerExtensions.GenerateUserPageQuery(skip, take, sortField, "");
        query.Should().Be($@"
SELECT u.Id, u.Name, u.LastActivityDate, u.IsAdministrator, u.IsHidden, u.IsDisabled
	, SUM(CASE WHEN uc.MediaType = 'Movie' THEN 1 ELSE 0 END) AS MovieViewCount
	, SUM(CASE WHEN uc.MediaType = 'Episode' THEN 1 ELSE 0 END) AS EpisodeViewCount
	, SUM(CASE WHEN uc.MediaType = 'Episode' OR uc.MediaType = 'Movie' THEN 1 ELSE 0 END) AS TotalViewCount
FROM MediaServerUsers AS u
LEFT JOIN MediaServerUserView AS uc ON (u.Id = uc.UserId)
GROUP BY u.Id
LIMIT {take} OFFSET {skip}");
    }
}