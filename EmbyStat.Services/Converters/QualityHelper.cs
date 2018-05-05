using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common;

namespace EmbyStat.Services.Converters
{
    public static class QualityHelper
    {
        public static string ConvertToQualityString(int? number)
        {
            if (number == null)
            {
                return Constants.LOWEST;
            }

            if (3800 <= number)
            {
                return Constants.FOURK;
            }

            if (2500 <= number)
            {
                return Constants.THOUSANDFOURFOURP;
            }

            if (1900 <= number)
            {
                return Constants.FULLHD;
            }

            if (1260 <= number)
            {
                return Constants.HDREADY;
            }

            if (700 <= number)
            {
                return Constants.FOURHUNDERDEIGHTY;
            }

            return Constants.LOWEST;
        }
    }
}
