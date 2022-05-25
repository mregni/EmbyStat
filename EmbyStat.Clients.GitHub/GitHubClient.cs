using System.Threading.Tasks;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.GitHub.Models;
using EmbyStat.Configuration;
using EmbyStat.Configuration.Interfaces;

namespace EmbyStat.Clients.GitHub;

public class GitHubClient : IGitHubClient
{
    private readonly Config _config;
    private readonly IRefitHttpClientFactory<IGitHubApi> _refitFactory;

    public GitHubClient(IConfigurationService configurationService, IRefitHttpClientFactory<IGitHubApi> refitFactory)
    {
        _refitFactory = refitFactory;
        _config = configurationService.Get();
    }

    public async Task<ReleaseObject[]> GetGithubVersions()
    {
        var client = _refitFactory.CreateClient(_config.SystemConfig.Updater.GitHubUrl);
        return await client.GetReleases();
    }
}