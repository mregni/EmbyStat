using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;

namespace Tests.Unit.Builders
{
    public class MovieBuilder
    {
        private Movie movie { get; set; }

        public MovieBuilder()
        {
            movie = new Movie
            {
                CommunityRating = (float) 1.7,
                Id = 0,
                Name = "The lord of the rings",
                PremiereDate = new DateTime(2002, 4, 2, 0, 0, 0),
                DateCreated = new DateTime(2018, 1, 1, 0, 0, 0),
                OfficialRating = "R",
                RunTimeTicks = 120000000000,
                Primary = "primaryImage",
                IMDB = "0001",
                Genres = new List<string> {"id1"},
                People = new List<ExtraPerson>
                    {new ExtraPerson {Id = Guid.NewGuid().ToString(), Name = "Gimli", Type = "Actor"}}
            };
        }

        public MovieBuilder AddName(string title)
        {
            movie.Name = title;
            return this;
        }

        public MovieBuilder AddId(int id)
        {
            movie.Id = id;
            return this;
        }

        public MovieBuilder AddImdb(string id)
        {
            movie.IMDB = id;
            return this;
        }

        public MovieBuilder AddPrimaryImage(string image)
        {
            movie.Primary = image;
            return this;
        }

        public MovieBuilder AddOfficialRating(string rating)
        {
            movie.OfficialRating = rating;
            return this;
        }

        public MovieBuilder AddCommunityRating(float rating)
        {
            movie.CommunityRating = rating;
            return this;
        }

        public MovieBuilder AddPremiereDate(DateTime date)
        {
            movie.PremiereDate = date;
            return this;
        }

        public MovieBuilder AddGenres(params string[] genres)
        {
            movie.Genres = genres.ToList();
            return this;
        }

        public MovieBuilder AddRunTimeTicks(int hours, int minute, int seconds)
        {
            var ticks = new TimeSpan(hours, minute, seconds);
            movie.RunTimeTicks = ticks.Ticks;
            return this;
        }

        public MovieBuilder AddPerson(ExtraPerson person)
        {
            movie.People.Add(person);
            return this;
        }

        public Movie Build()
        {
            return movie;
        }
    }
}
