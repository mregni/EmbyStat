using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Core.DataStore.Seeds;

public static class MediaServerSeed
{
    public static readonly MediaServerStatus Status = new() {Id = new Guid("e55668a1-6a81-47ba-882d-738347e7e9cd"), MissedPings = 0};
}