using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Common.Extensions
{
    public static class ShowExtension
    {
        public static int GetNonSpecialEpisodeCount(this Show show, bool includeMissing)
        {
            var specialSeason = show.Seasons.SingleOrDefault(x => x.IndexNumber == 0);
            var list = includeMissing ? show.Episodes : show.Episodes.Where(x => x.LocationType == LocationType.Disk);

            return specialSeason != null
                ? list.Count(x => x.ParentId != specialSeason.Id.ToString())
                : list.Count();
        }

        public static int GetSpecialEpisodeCount(this Show show, bool includeMissing)
        {
            var specialSeason = show.Seasons.SingleOrDefault(x => x.IndexNumber == 0);
            var list = includeMissing ? show.Episodes : show.Episodes.Where(x => x.LocationType == LocationType.Disk);

            return specialSeason != null
                ? list.Count(x => x.ParentId == specialSeason.Id.ToString())
                : 0;
        }

        public static int GetNonSpecialSeasonCount(this Show show)
        {
            var specialSeason = show.Seasons.SingleOrDefault(x => x.IndexNumber == 0);
            return specialSeason != null
                ? show.Seasons.Count(x => x.Id != specialSeason.Id)
                : show.Seasons.Count;
        }
    }
}
