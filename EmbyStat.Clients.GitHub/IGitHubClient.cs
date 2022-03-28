using System;
using System.Threading.Tasks;
using EmbyStat.Clients.GitHub.Models;
using EmbyStat.Common.Enums;

namespace EmbyStat.Clients.GitHub
{
    public interface IGitHubClient
    {
        Task<ReleaseObject[]> GetGithubVersions(Version minVersion, string assetFileName, UpdateTrain updateTrain);
    }
}
