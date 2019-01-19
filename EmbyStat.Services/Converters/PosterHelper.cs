using System;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Converters
{
    public static class PosterHelper
    {
        public static MoviePoster ConvertToMoviePoster(Movie movie, string title)
        {
            return new MoviePoster
            {
                Title = title,
                Name = movie.Name,
                CommunityRating = String.Format("{0:0.0}", movie.CommunityRating ?? 0),
                MediaId = movie.Id,
                OfficialRating = movie.OfficialRating,
                Tag = movie.Primary,
                DurationMinutes = Math.Floor(new TimeSpan(movie.RunTimeTicks ?? 0).TotalMinutes),
                Year = movie.PremiereDate?.Year ?? 0
            };
        }

        public static ShowPoster ConvertToShowPoster(Show show, string title)
        {
            return new ShowPoster
            {
                Title = title,
                Name = show.Name,
                CommunityRating = String.Format("{0:0.0}", show.CommunityRating ?? 0),
                MediaId = show.Id,
                OfficialRating = show.OfficialRating,
                Tag = show.Primary,
                Year = show.PremiereDate?.Year ?? 0
            };
        }

        public static PersonPoster ConvertToPersonPoster(Person person, string title)
        {
            return new PersonPoster
            {
                MediaId = person.Id,
                Name = person.Name,
                BirthDate = person.BirthDate,
                MovieCount = person.MovieCount,
                EpisodeCount = person.seriesCount,
                Title = title,
                HasTitle = true,
                Tag = person.Primary
            };
        }
    }
}
