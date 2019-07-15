using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.GitHub.Models;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Common.Net;
using Microsoft.Extensions.Options;

namespace EmbyStat.Clients.GitHub
{
    public class GithubClient : IGithubClient
    {
        private readonly IAsyncHttpClient _httpClient;
        private readonly AppSettings _appSettings;

        public GithubClient(IAsyncHttpClient httpClient, IOptions<AppSettings> appSettings)
        {
            _httpClient = httpClient;
            _appSettings = appSettings.Value;
        }

        public async Task<ReleaseObject[]> GetGithubVersionsAsync(Version minVersion, string assetFileName, UpdateTrain updateTrain, CancellationToken cancellationToken)
        {
            var options = new HttpRequest
            {
                Url = _appSettings.Updater.GithubUrl,
                Method = "GET",
                RequestContentType = "application/json",
                CancellationToken = cancellationToken,
                UserAgent = "EmbyStat/1.0"
            };

            using (var stream = await _httpClient.SendAsync(options))
            {
                return JsonSerializerExtentions.DeserializeFromStream<ReleaseObject[]>(stream);
            }
        }
    }
}
