using EmbyStat.Common;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Core.DataStore.Seeds;

public static class JobSeed
{
    public static readonly Job[] Jobs =
    {
        new(Constants.JobIds.CheckUpdateId, Constants.LogPrefix.CheckUpdateJob, "0 */12 * * *"),
        new(Constants.JobIds.SmallSyncId, Constants.LogPrefix.SmallMediaServerSyncJob, "0 2 * * *"),
        new(Constants.JobIds.ShowSyncId, Constants.LogPrefix.ShowSyncJob, "0 3 * * *"),
        new(Constants.JobIds.PingEmbyId, Constants.LogPrefix.PingMediaServerJob, "*/5 * * * *"),
        new(Constants.JobIds.DatabaseCleanupId, Constants.LogPrefix.DatabaseCleanupJob, "0 4 * * *"),
        new(Constants.JobIds.MovieSyncId, Constants.LogPrefix.MovieSyncJob, "0 2 * * *")
    };
}