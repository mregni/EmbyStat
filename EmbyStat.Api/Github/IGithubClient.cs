using System;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.Github.Models;

namespace EmbyStat.Api.Github
{
    public interface IGithubClient
    {
        Task<UpdateResult> CheckIfUpdateAvailable(Version minVersion, string assetFileName, CancellationToken cancellationToken);
    }
}
