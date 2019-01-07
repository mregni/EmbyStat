using System;

namespace EmbyStat.Common.Extentions
{
    public static class RoundExtention
    {
        public static int? RoundToFive(this DateTime? date)
        {
            if (date.HasValue)
            {
                var result = (int)Math.Floor((double)date.Value.Year / 5) * 5;
                return result;
            }

            return null;
        }

        public static int? RoundToFive(this double date)
        {
            return (int)Math.Floor((double)date *20) * 5;
        }

        public static double? RoundToHalf(this float? rating)
        {
            if (rating.HasValue)
            {
                return Math.Round((double)rating.Value * 2, MidpointRounding.AwayFromZero) / 2;
            }

            return null;
        }


    }
}
