namespace EmbyStat.Common.Converters
{
    public static class QualityConverter
    {
        public static string ConvertToQualityString(this int? number)
        {
            if (number == null)
            {
                return Constants.Dvd;
            }

            if (3800 <= number)
            {
                return Constants.FourK;
            }

            if (2500 <= number)
            {
                return Constants.Qhd;
            }

            if (1900 <= number)
            {
                return Constants.FullHd;
            }

            if (1260 <= number)
            {
                return Constants.HdReady;
            }

            if (700 <= number)
            {
                return Constants.FourHunderdEighty;
            }

            return Constants.Dvd;
        }
    }
}
