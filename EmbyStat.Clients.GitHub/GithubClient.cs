using System;
using EmbyStat.Clients.GitHub.Models;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Settings;
using Microsoft.Extensions.Options;
using RestSharp;

namespace EmbyStat.Clients.GitHub
{
    public class GithubClient : IGithubClient
    {
        private IRestClient RestClient { get;  }
        private readonly AppSettings _appSettings;

        public GithubClient(IOptions<AppSettings> appSettings, IRestClient client)
        {
            RestClient = client.Initialize();
            _appSettings = appSettings.Value;
        }

        public ReleaseObject[] GetGithubVersions(Version minVersion, string assetFileName, UpdateTrain updateTrain)
        {
            var request = new RestRequest(_appSettings.Updater.GithubUrl, Method.GET);
            var result = RestClient.Execute<ReleaseObject[]>(request);
            return result.Data;
        }
    }
}
