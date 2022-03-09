using System;

namespace EmbyStat.Common.Extensions
{
    public static class RoundExtension
    {
        public static int? RoundToFiveYear(this DateTime? date)
        {
            if (date.HasValue)
            {
                var result = (int)Math.Floor((double)date.Value.Year / 5) * 5;
                return result;
            }

            return null;
        }

        public static int? RoundToFive(this double count)
        {
            return (int)Math.Floor(count * 20) * 5;
        }

        public static double? RoundToHalf(this decimal? rating)
        {
            if (rating.HasValue)
            {
                return Math.Round((double)rating.Value * 2, MidpointRounding.AwayFromZero) / 2;
            }

            return null;
        }
    }
}
