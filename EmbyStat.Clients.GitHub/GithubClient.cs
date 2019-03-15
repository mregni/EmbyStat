using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.EmbyClient.Net;
using EmbyStat.Clients.Github.Models;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models.Settings;
using Microsoft.Extensions.Options;

namespace EmbyStat.Clients.Github
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

        public async Task<UpdateResult> CheckIfUpdateAvailable(Version minVersion, string assetFileName, UpdateTrain updateTrain, CancellationToken cancellationToken)
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
                var obj = JsonSerializerExtentions.DeserializeFromStream<ReleaseObject[]>(stream);
                return CheckForUpdateResult(obj, minVersion, updateTrain, assetFileName);
            }
        }

        private UpdateResult CheckForUpdateResult(ReleaseObject[] obj, Version minVersion, UpdateTrain updateTrain, string assetFilename)
        {
            if (updateTrain == UpdateTrain.Release)
            {
                obj = obj.Where(i => !i.PreRelease).ToArray();
            }
            else if (updateTrain == UpdateTrain.Beta)
            {
                obj = obj.Where(i => i.PreRelease && i.Name.Contains(_appSettings.Updater.BetaString, StringComparison.OrdinalIgnoreCase)).ToArray();
            }
            else if (updateTrain == UpdateTrain.Dev)
            {
                obj = obj.Where(i => i.PreRelease && i.Name.Contains(_appSettings.Updater.DevString, StringComparison.OrdinalIgnoreCase)).ToArray();
            }

            var availableUpdate = obj
                .Select(i => CheckForUpdateResult(i, minVersion, assetFilename))
                .Where(i => i != null)
                .OrderByDescending(i => Version.Parse(i.AvailableVersion))
                .FirstOrDefault();

            return availableUpdate ?? new UpdateResult();
        }
        private UpdateResult CheckForUpdateResult(ReleaseObject obj, Version minVersion, string assetFilename)
        {
            var versionString = CleanUpVersionString(obj.TagName);
            
            if (!Version.TryParse(versionString, out var version))
            {
                return null;
            }

            if (version < minVersion)
            {
                return null;
            }

            var asset = (obj.Assets ?? new List<Asset>()).FirstOrDefault(i => IsAsset(i, assetFilename, obj.TagName));
            if (asset == null)
            {
                return null;
            }

            return new UpdateResult
            {
                AvailableVersion = version.ToString(),
                IsUpdateAvailable = version > minVersion,
                Package = new PackageInfo
                {
                    classification = obj.PreRelease
                        ? (obj.Name.Contains(_appSettings.Updater.DevString, StringComparison.OrdinalIgnoreCase) ? UpdateTrain.Dev : UpdateTrain.Beta)
                        : UpdateTrain.Release,
                    name = asset.name,
                    sourceUrl = asset.browser_download_url,
                    versionStr = version.ToString(),
                    infoUrl = obj.HtmlUrl
                }
            };
        }

        private bool IsAsset(Asset asset, string assetFilename, string version)
        {
            var downloadFilename = Path.GetFileName(asset.browser_download_url) ?? string.Empty;
            assetFilename = assetFilename.Replace("{version}", version);

            if (downloadFilename.IndexOf(assetFilename, StringComparison.OrdinalIgnoreCase) != -1)
            {
                return true;
            }

            return string.Equals(assetFilename, downloadFilename, StringComparison.OrdinalIgnoreCase);
        }

        private string CleanUpVersionString(string version)
        {
            return version.Replace(_appSettings.Updater.BetaString, "").Replace(_appSettings.Updater.DevString, "");
        }
    }
}
