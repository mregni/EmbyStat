using System;
using System.Threading.Tasks;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Clients.GitHub.Models;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Settings;
using Microsoft.Extensions.Options;

namespace EmbyStat.Clients.GitHub
{
    public class GitHubClient : IGitHubClient
    {
        private readonly AppSettings _appSettings;
        private readonly IRefitHttpClientFactory<IGitHubApi> _refitFactory;

        public GitHubClient(IOptions<AppSettings> appSettings, IRefitHttpClientFactory<IGitHubApi> refitFactory)
        {
            _refitFactory = refitFactory;
            _appSettings = appSettings.Value;
        }

        public async Task<ReleaseObject[]> GetGithubVersions(Version minVersion, string assetFileName, UpdateTrain updateTrain)
        {
            var client = _refitFactory.CreateClient(_appSettings.Updater.GitHubUrl);
            return await client.GetReleases();
        }
    }
}
