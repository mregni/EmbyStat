using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Show;

namespace EmbyStat.Common.Extensions
{
    public static class ShowExtension
    {
        /// <summary>
        /// GetEpisodeCount will return the correct episode on a show. Episodes spanning more then 1 episode will count as multiple
        /// </summary>
        /// <param name="show">Show for which episode count is calculated.</param>
        /// <param name="specialSeason">Set to true if only the special season needs to be counted. False will ignore the special season.</param>
        /// <param name="locations">Location is used to check for disk or virtual (or both) episodes.</param>
        /// <returns>Total episode count</returns>
        public static int GetEpisodeCount(this Show show, bool specialSeason, params LocationType[] locations)
        {
            var query = show.Episodes.Where(x => locations.Any(y => y == x.LocationType));
            query = specialSeason
                ? query.Where(x => x.SeasonIndexNumber == 0)
                : query.Where(x => x.SeasonIndexNumber != 0);
            var list = query.ToList();

            var count = list.Count(x => x.IndexNumberEnd == null);
            count += list.Where(x => x.IndexNumberEnd != null && x.IndexNumber != null).Sum(x => x.IndexNumberEnd.Value - x.IndexNumber.Value + 1);

            return count;
        }

        /// <summary>
        /// Returns the total season count for a show
        /// </summary>
        /// <param name="show">Show for which season count is calculated.</param>
        /// <param name="includeSpecial">Include the special season for the total season count number</param>
        /// <returns>Total season count</returns>
        public static int GetSeasonCount(this Show show, bool includeSpecial)
        {
            return includeSpecial
                ? show.Seasons.Count()
                : show.Seasons.Count(x => x.IndexNumber != 0);
        }

        /// <summary>
        /// Returns a virtualEpisode object for all episodes that are missing in a show
        /// </summary>
        /// <param name="show">Show for which the virtual episodes are generated</param>
        /// <returns>List of VirtualEpisode that contains the episode and show information.</returns>
        public static IEnumerable<VirtualEpisode> GetMissingEpisodes(this Show show)
        {
            return show.Episodes
                .Where(x => x.LocationType == LocationType.Virtual && x.SeasonIndexNumber != 0)
                .Select(x => new VirtualEpisode(x))
                .ToList();
        }

        /// <summary>
        /// Calculates the total Mb size of a show by making the total SUM of the first MediaSource for each Episode
        /// </summary>
        /// <param name="show">Show for which the total size needs to be calculated</param>
        /// <returns>Total space used for the show in Mb </returns>
        public static double GetShowSize(this Show show)
        {
            return show.Episodes
                .Where(x => x.LocationType == LocationType.Disk)
                .Sum(x => x.MediaSources.FirstOrDefault()?.SizeInMb ?? 0);
        }

        /// <summary>
        /// Checks if the show needs a resync when external sync is failed or never happened
        /// </summary>
        /// <param name="show">Show for which the check is performed</param>
        /// <returns>True or false if the a resync is needed or not</returns>
        public static bool NeedsShowSync(this Show show)
        {
            return !show.ExternalSynced || show.ExternalSyncFailed;
        }

        /// <summary>
        /// Checks if any episodes are changed since the last sync for a show
        /// </summary>
        /// <param name="show">New show data from the external server</param>
        /// <param name="oldShow">Internal show data on which a comparison is required</param>
        /// <returns>True or false if episodes have changed</returns>
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
