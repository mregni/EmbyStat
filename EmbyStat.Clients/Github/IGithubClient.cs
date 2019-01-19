using System;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.Github.Models;
using EmbyStat.Common.Enums;

namespace EmbyStat.Clients.Github
{
    public interface IGithubClient
    {
        Task<UpdateResult> CheckIfUpdateAvailable(Version minVersion, string assetFileName, UpdateTrain updateTrain, CancellationToken cancellationToken);
    }
}
