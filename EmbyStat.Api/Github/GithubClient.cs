using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.Github.Models;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using MediaBrowser.Model.Serialization;
using EmbyStat.Api.EmbyClient.Net;

namespace EmbyStat.Api.Github
{
    public class GithubClient : IGithubClient
    {
        private readonly IAsyncHttpClient _httpClient;
        private readonly IJsonSerializer _jsonSerializer;

        public GithubClient(IAsyncHttpClient httpClient, IJsonSerializer jsonSerializer)
        {
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
        }

        public async Task<CheckForUpdateResult> CheckIfUpdateAvailable(Version minVersion, string assetFileName, string packageName, string targetFilename, CancellationToken cancellationToken)
        {
            var options = new HttpRequest
            {
                Url = Constants.GitHub.GitHubReleaseUrl,
                Method = "GET",
                RequestContentType = "application/json",
                CancellationToken = cancellationToken,
                UserAgent = "EmbyStat/1.0"
            };

            using (var stream = await _httpClient.SendAsync(options))
            {
                var obj = _jsonSerializer.DeserializeFromStream<ReleaseObject[]>(stream);
                return CheckForUpdateResult(obj, minVersion, UpdateLevel.Beta, assetFileName, packageName, targetFilename);
            }
        }

        public Stream DownloadUpdate()
        {
            throw new NotImplementedException();
        }

        private CheckForUpdateResult CheckForUpdateResult(ReleaseObject[] obj, Version minVersion, UpdateLevel updateLevel, string assetFilename, string packageName, string targetFilename)
        {
            if (updateLevel == UpdateLevel.Release)
            {
                obj = obj.Where(i => !i.prerelease).ToArray();
            }
            else if (updateLevel == UpdateLevel.Beta)
            {
                obj = obj.Where(i => i.prerelease && i.name.EndsWith("-beta", StringComparison.OrdinalIgnoreCase)).ToArray();
            }
            else if (updateLevel == UpdateLevel.Dev)
            {
                obj = obj.Where(i => i.prerelease && i.name.EndsWith("-dev", StringComparison.OrdinalIgnoreCase)).ToArray();
            }

            var availableUpdate = obj
                .Select(i => CheckForUpdateResult(i, minVersion, assetFilename, packageName, targetFilename))
                .Where(i => i != null)
                .OrderByDescending(i => Version.Parse(i.AvailableVersion))
                .FirstOrDefault();

            return availableUpdate ?? new CheckForUpdateResult
            {
                IsUpdateAvailable = false
            };
        }
        private CheckForUpdateResult CheckForUpdateResult(ReleaseObject obj, Version minVersion, string assetFilename, string packageName, string targetFilename)
        {
            var versionString = obj.tag_name;
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

            return new CheckForUpdateResult
            {
                AvailableVersion = version.ToString(),
                IsUpdateAvailable = version > minVersion,
                Package = new PackageInfo
                {
                    classification = obj.prerelease
                        ? (obj.name.EndsWith("-dev", StringComparison.OrdinalIgnoreCase) ? UpdateLevel.Dev : UpdateLevel.Beta)
                        : UpdateLevel.Release,
                    name = packageName,
                    sourceUrl = asset.browser_download_url,
                    targetFilename = targetFilename,
                    versionStr = version.ToString(),
                    requiredVersionStr = "1.0.0",
                    description = obj.body,
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
    }
}
