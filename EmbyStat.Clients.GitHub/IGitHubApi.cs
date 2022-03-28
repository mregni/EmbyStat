using System.Threading.Tasks;
using EmbyStat.Clients.GitHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Clients.GitHub;

public interface IGitHubApi
{
    /// <summary>
    /// Fetches all releases from the public EmbyStat repo.
    /// </summary>
    /// <returns>Array of type <see cref="ReleaseObject"/></returns>
    [HttpGet("/releases")]
    Task<ReleaseObject[]> GetReleases();
}