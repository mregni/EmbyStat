using System;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Show;
using EmbyStat.Common.SqLite.Shows;

namespace EmbyStat.Common.Extensions;

public static class VirtualEpisodeExtensions
{
    public static SqlEpisode ConvertToVirtualEpisode(this VirtualEpisode episode, SqlSeason season)
    {
        return new SqlEpisode
        {
            Id = Guid.NewGuid().ToString(),
            Name = episode.Name,
            LocationType = LocationType.Virtual,
            IndexNumber = episode.EpisodeNumber,
            SeasonId = season.Id,
            PremiereDate = episode.FirstAired ?? new DateTime()
        };
    }
}