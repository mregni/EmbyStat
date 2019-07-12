using System;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.GitHub.Models;
using EmbyStat.Common.Enums;

namespace EmbyStat.Clients.GitHub
{
    public interface IGithubClient
    {
        Task<ReleaseObject[]> GetGithubVersionsAsync(Version minVersion, string assetFileName, UpdateTrain updateTrain, CancellationToken cancellationToken);
    }
}
