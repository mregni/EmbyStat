using System;
using EmbyStat.Clients.GitHub.Models;
using EmbyStat.Common.Enums;

namespace EmbyStat.Clients.GitHub
{
    public interface IGithubClient
    {
        ReleaseObject[] GetGithubVersions(Version minVersion, string assetFileName, UpdateTrain updateTrain);
    }
}
