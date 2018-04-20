using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Stats;

namespace EmbyStat.Common.Helpers
{
    public static class PosterHelper
    {
        public static Poster ConvertToPoster(Movie movie, string title)
        {
            return new Poster
            {
                Title = title,
                Name = movie.Name,
                CommunityRating = Math.Floor(movie.CommunityRating ?? 0).ToString(CultureInfo.InvariantCulture),
                MediaId = movie.Id,
                OfficialRating = movie.OfficialRating,
                Tag = movie.Primary,
                DurationMinutes = Math.Floor(new TimeSpan(movie.RunTimeTicks ?? 0).TotalMinutes)
            };
        }
    }
}
