using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Show;

namespace EmbyStat.Common.Extensions
{
    public static class ShowExtension
    {
        public static int GetNonSpecialEpisodeCount(this Show show, bool includeMissing)
        {
            var specialSeason = show.Seasons.Where(x => x.IndexNumber == 0).ToList();
            var list = includeMissing ? show.Episodes : show.Episodes.Where(x => x.LocationType == LocationType.Disk);

            return specialSeason.Any()
                ? list.Count(x => specialSeason.All(y => y.Id.ToString() != x.ParentId))
                : list.Count();
        }

        public static int GetNonSpecialSeasonCount(this Show show)
        {
            var specialSeason = show.Seasons.Where(x => x.IndexNumber == 0).ToList();
            return specialSeason.Any()
                ? show.Seasons.Count(x => specialSeason.All(y => y.Id != x.Id))
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
                .Select(x => new VirtualEpisode(x, show.Seasons.First(y => y.Id.ToString() == x.ParentId)));
        }

        public static double GetShowSize(this Show show)
        {
            return show.Episodes
                .Where(x => x.LocationType == LocationType.Disk)
                .Sum(x => x.MediaSources.First().SizeInMb);
        }

        public static bool NeedsShowSync(this Show show)
        {
            return !show.TvdbSynced || show.TvdbFailed;
        }

        public static bool HasShowChangedEpisodes(this Show show, Show oldShow)
        {
            if (oldShow == null)
            {
                return true;
            }

            return !oldShow.Episodes.Select(x => x.Id).AreListEqual(show.Episodes.Select(x => x.Id));
        }
    }
}
