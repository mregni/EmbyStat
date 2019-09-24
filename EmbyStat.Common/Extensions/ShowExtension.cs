using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Show;

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

        public static int GetNonSpecialSeasonCount(this Show show)
        {
            var specialSeason = show.Seasons.SingleOrDefault(x => x.IndexNumber == 0);
            return specialSeason != null
                ? show.Seasons.Count(x => x.Id != specialSeason.Id)
                : show.Seasons.Count;
        }

        public static int GetMissingEpisodeCount(this Show show)
        {
            return show.Episodes.Count(x => x.LocationType == LocationType.Virtual);
        }

        public static IEnumerable<VirtualEpisode> GetMissingEpisodes(this Show show)
        {
            return show.Episodes
                .Where(x => x.LocationType == LocationType.Virtual)
                .Select(x => new VirtualEpisode(x, show.Seasons.Single(y => y.Id.ToString() == x.ParentId)));
        }

        public static double GetShowSize(this Show show)
        {
            return show.Episodes
                .Where(x => x.LocationType == LocationType.Disk)
                .Sum(x => x.MediaSources.First().SizeInMb);
        }
    }
}
