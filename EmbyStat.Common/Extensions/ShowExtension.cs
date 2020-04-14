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
            var list = includeMissing ? show.Episodes.ToList() : show.Episodes.Where(x => x.LocationType == LocationType.Disk).ToList();
            
            list = specialSeason.Any() ? list.Where(x => specialSeason.All(y => y.Id.ToString() != x.ParentId)).ToList() : list;

            var count = list.Count(x => x.IndexNumberEnd == null);
            count += list.Where(x => x.IndexNumberEnd != null && x.IndexNumber != null).Sum(x => x.IndexNumberEnd.Value - x.IndexNumber.Value + 1);

            return count;
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
                .Select(x => new VirtualEpisode(x, show.Seasons.First(y => y.Id == x.ParentId)))
                .ToList();
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
