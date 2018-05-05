using System;
using System.Linq;
using EmbyStat.Common.Models;
using EmbyStat.Services.Models.Stat;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

namespace EmbyStat.Services.Converters
{
    public static class PosterHelper
    {
        public static Poster ConvertToPoster(Movie movie, string title)
        {
            return new Poster
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

        public static PersonPoster ConvertToPersonPoster(Person person, string title)
        {
            return new PersonPoster
            {
                MediaId = person.Id,
                Name = person.Name,
                BirthDate = person.BirthDate,
                ChildCount = person.ChildCount,
                MovieCount = person.MovieCount,
                EpisodeCount = person.EpisodeCount,
                Title = title,
                HasTitle = true,
                Tag = person.Primary
            };
        }
    }
}
