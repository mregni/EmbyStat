using System;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Seeds;

public static class MediaServerSeed
{
    public static readonly MediaServerStatus Status = new MediaServerStatus {Id = new Guid("e55668a1-6a81-47ba-882d-738347e7e9cd"), MissedPings = 0};
}