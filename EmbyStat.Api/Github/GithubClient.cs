using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.Github.Models;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Api.EmbyClient.Net;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models.Settings;
using Microsoft.Extensions.Options;

namespace EmbyStat.Api.Github
{
    public class GithubClient : IGithubClient
    {
        private readonly IAsyncHttpClient _httpClient;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly AppSettings _appSettings;

        public GithubClient(IAsyncHttpClient httpClient, IJsonSerializer jsonSerializer, IOptions<AppSettings> appSettings)
        {
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
            _appSettings = appSettings.Value;
        }

        public async Task<UpdateResult> CheckIfUpdateAvailable(Version minVersion, string assetFileName,  CancellationToken cancellationToken)
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
                var obj = _jsonSerializer.DeserializeFromStream<ReleaseObject[]>(stream);
                return CheckForUpdateResult(obj, minVersion, UpdateTrain.Beta, assetFileName);
            }
        }

        private UpdateResult CheckForUpdateResult(ReleaseObject[] obj, Version minVersion, UpdateTrain updateTrain, string assetFilename)
        {
            if (updateTrain == UpdateTrain.Release)
            {
                obj = obj.Where(i => !i.prerelease).ToArray();
            }
            else if (updateTrain == UpdateTrain.Beta)
            {
                obj = obj.Where(i => i.prerelease && i.name.Contains(_appSettings.Updater.BetaString, StringComparison.OrdinalIgnoreCase)).ToArray();
            }
            else if (updateTrain == UpdateTrain.Dev)
            {
                obj = obj.Where(i => i.prerelease && i.name.Contains(_appSettings.Updater.DevString, StringComparison.OrdinalIgnoreCase)).ToArray();
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
            var versionString = CleanUpVersionString(obj.tag_name);
            versionString = versionString.Replace("-beta", "").Replace("-dev", "");

            if (!Version.TryParse(versionString, out var version))
            {
                return null;
            }

            if (version < minVersion)
            {
                return null;
            }

            var asset = (obj.assets ?? new List<Asset>()).FirstOrDefault(i => IsAsset(i, assetFilename, versionString));
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
                    classification = obj.prerelease
                        ? (obj.name.Contains(_appSettings.Updater.DevString, StringComparison.OrdinalIgnoreCase) ? UpdateTrain.Dev : UpdateTrain.Beta)
                        : UpdateTrain.Release,
                    name = asset.name,
                    sourceUrl = asset.browser_download_url,
                    versionStr = version.ToString(),
                    infoUrl = obj.html_url
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
