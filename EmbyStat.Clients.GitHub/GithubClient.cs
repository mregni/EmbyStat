using System;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.GitHub.Models;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Common.Net;
using Microsoft.Extensions.Options;
using RestSharp;

namespace EmbyStat.Clients.GitHub
{
    public class GithubClient : IGithubClient
    {
        private IRestClient Client { get; set; }
        private readonly AppSettings _appSettings;

        public GithubClient(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            Client = new RestClient().UseSerializer(() => new JsonNetSerializer());
            Client.UserAgent = "EmbyStat/1.0";
        }

        public async Task<ReleaseObject[]> GetGithubVersionsAsync(Version minVersion, string assetFileName, UpdateTrain updateTrain, CancellationToken cancellationToken)
        {
            var request = new RestRequest(_appSettings.Updater.GithubUrl, Method.GET);
            var result = await Client.ExecuteTaskAsync<ReleaseObject[]>(request, cancellationToken);
            return result.Data;
        }
    }
}
