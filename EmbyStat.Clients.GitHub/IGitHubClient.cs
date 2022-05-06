using System.Threading.Tasks;
using EmbyStat.Clients.GitHub.Models;

namespace EmbyStat.Clients.GitHub;

public interface IGitHubClient
{
    Task<ReleaseObject[]> GetGithubVersions();
}