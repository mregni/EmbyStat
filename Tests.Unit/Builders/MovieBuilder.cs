using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using MediaBrowser.Model.Entities;

namespace Tests.Unit.Builders
{
    public class MovieBuilder
    {
        private readonly Movie _movie;

        public MovieBuilder(int id)
        {
            _movie = new Movie
            {
                CommunityRating = (float) 1.7,
                Id = id,
                Name = "The lord of the rings",
                PremiereDate = new DateTime(2002, 4, 2, 0, 0, 0),
                DateCreated = new DateTime(2018, 1, 1, 0, 0, 0),
                OfficialRating = "R",
                RunTimeTicks = 120000000000,
                Primary = "primaryImage",
                IMDB = "0001",
                Video3DFormat = null,
                Genres = new[] {"id1"},
                People = new[] {new ExtraPerson {Id = Guid.NewGuid().ToString(), Name = "Gimli", Type = PersonType.Actor}},
                MediaSources = new List<MediaSource> { new MediaSource { SizeInMb = 1001 } },
                CollectionId = "1"
            };
        }

        public MovieBuilder AddCreateDate(DateTime date)
        {
            _movie.DateCreated = date;
            return this;
        }

        public MovieBuilder AddName(string title)
        {
            _movie.Name = title;
            return this;
        }

        public MovieBuilder AddImdb(string id)
        {
            _movie.IMDB = id;
            return this;
        }

        public MovieBuilder AddPrimaryImage(string image)
        {
            _movie.Primary = image;
            return this;
        }

        public MovieBuilder AddOfficialRating(string rating)
        {
            _movie.OfficialRating = rating;
            return this;
        }

        public MovieBuilder AddCommunityRating(float rating)
        {
            _movie.CommunityRating = rating;
            return this;
        }

        public MovieBuilder AddPremiereDate(DateTime date)
        {
            _movie.PremiereDate = date;
            return this;
        }

        public MovieBuilder AddGenres(params string[] genres)
        {
            _movie.Genres = genres;
            return this;
        }

        public MovieBuilder AddRunTimeTicks(int hours, int minute, int seconds)
        {
            var ticks = new TimeSpan(hours, minute, seconds);
            _movie.RunTimeTicks = ticks.Ticks;
            return this;
        }

        public MovieBuilder AddPerson(ExtraPerson person)
        {
            var list = _movie.People.ToList();
            list.Add(person);
            _movie.People = list.ToArray();
            return this;
        }

        public MovieBuilder AddVideo3DFormat(Video3DFormat format)
        {
            _movie.Video3DFormat = format;
            return this;
        }

        public MovieBuilder AddCollectionId(string id)
        {
            _movie.CollectionId = id;
            return this;
        }

        public Movie Build()
        {
            return _movie;
        }
    }
}
