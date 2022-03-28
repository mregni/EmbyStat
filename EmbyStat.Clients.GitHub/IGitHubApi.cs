using System.Threading.Tasks;
using EmbyStat.Clients.GitHub.Models;
using Refit;

namespace EmbyStat.Clients.GitHub;

public interface IGitHubApi
{
    /// <summary>
    /// Fetches all releases from the public EmbyStat repo.
    /// </summary>
    /// <returns>Array of type <see cref="ReleaseObject"/></returns>
    [Get("/releases")]
    Task<ReleaseObject[]> GetReleases();
}