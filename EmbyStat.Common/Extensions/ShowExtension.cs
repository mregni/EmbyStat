using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
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
                ? show.Seasons.Count
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
        /// Adds new items to current list or if Id is already present in the list, it will update all properties to new values.
        /// </summary>
        /// <typeparam name="T">Media type</typeparam>
        /// <param name="list1">Base list</param>
        /// <param name="list2">List that will be merged to the base list</param>
        public static void Upsert<T>(this List<T> list1, List<T> list2) where T : Media
        {
            foreach (var item in list2)
            {
                if (list1.Any(n => n.Equals(item)))
                {
                    var obj = list1.First(n => n.Equals(item));
                    foreach (var pi in obj.GetType().GetProperties())
                    {
                        var v1 = pi.GetValue(obj, null);
                        var v2 = pi.GetValue(item, null);
                        var value = v1;
                        if (v2 != null && v1 != v2)
                        {
                            value = v2;
                        }
                        pi.SetValue(obj, value, null);
                    }
                }
                else
                {
                    list1.Add(item);
                }
            }
        }
    }
}
