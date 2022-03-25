using System;
using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.SqLite.Shows;

namespace EmbyStat.Common.Extensions;

public static class IntExtensions
{
    public static SqlSeason ConvertToVirtualSeason(this int indexNumber, SqlShow show)
    {
        return new SqlSeason
        {
            Id = Guid.NewGuid().ToString(),
            Name = indexNumber == 0 ? "Special" : $"Season {indexNumber}",
            ShowId = show.Id,
            Path = string.Empty,
            DateCreated = null,
            IndexNumber = indexNumber,
            IndexNumberEnd = indexNumber,
            PremiereDate = null,
            ProductionYear = null,
            SortName = indexNumber.ToString("0000"),
            LocationType = LocationType.Virtual,
            Episodes = new List<SqlEpisode>()
        };
    }
}