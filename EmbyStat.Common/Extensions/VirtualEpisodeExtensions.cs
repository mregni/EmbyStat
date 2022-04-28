using System;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Show;

namespace EmbyStat.Common.Extensions;

public static class VirtualEpisodeExtensions
{
    public static Episode ConvertToVirtualEpisode(this VirtualEpisode episode, Season season)
    {
        return new Episode
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