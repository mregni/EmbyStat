using System;

namespace EmbyStat.Common.Extensions;

public static class TimeZoneInfoExtensions
{
    public static string GetTimeZoneName()
    {
        string timeZone;
        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TZ")))
        {
            timeZone = Environment.GetEnvironmentVariable("TZ") ?? "Europe/Londen";
        }
        else
        {
            timeZone = TimeZoneInfo.TryConvertWindowsIdToIanaId(TimeZoneInfo.Local.Id, out var ianaId) 
                ? ianaId 
                : TimeZoneInfo.Local.Id;
        }

        return timeZone;
    }
}