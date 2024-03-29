﻿using System.Globalization;

namespace EmbyStat.Common.Extensions;

public static class DoubleExtensions
{
    public static string FormatToDotDecimalString(this double value)
    {
        return value
            .ToString(CultureInfo.CurrentCulture)
            .Replace(",", ".");
    }
}