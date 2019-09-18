using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using MediaBrowser.Model.Entities;

namespace Tests.Unit.Builders
{
    public class ShowBuilder
    {
        private readonly Show _show;

        public ShowBuilder(int id, string collectionId)
        {
            _show = new Show
            {
                Id = id,
                Path = "Path/To/Show",
                Banner = "banner.png",
                CollectionId = collectionId,
                CommunityRating = null,
                CumulativeRunTimeTicks = 19400287400000,
                DateCreated = new DateTimeOffset(2001, 01, 01, 0, 0, 0, new TimeSpan(0)),
                IMDB = "12345",
                Logo = "logo.jpg",
                MissingEpisodesCount = 0,
                Name = "Chuck",
                OfficialRating = "R",
                ParentId = collectionId,
                Primary = "primary.jpg",
                ProductionYear = 2001,
                RunTimeTicks = 12000000,
                SortName = "Chuck",
                Status = "Ended",
                TMDB = "12345",
                TVDB = "12345",
                Thumb = "thumb.jpg",
                TvdbFailed = false,
                TvdbSynced = false,
                PremiereDate = new DateTimeOffset(2001, 01, 01, 0, 0, 0, new TimeSpan(0)),
                People = new[] {new ExtraPerson {Id = Guid.NewGuid().ToString(), Name = "Gimli", Type = PersonType.Actor}},
                Genres = new[] { "Action" },
                Episodes = new List<Episode>
                {
                    new EpisodeBuilder(1, id, "1").Build(),
                    new EpisodeBuilder(2, id, "1").Build(),
                },
                Seasons = new List<Season>
                {
                    new SeasonBuilder(1, id.ToString()).Build()
                }
            };
        }

        public ShowBuilder AddName(string name)
        {
            _show.Name = name;
            _show.SortName = "0001 - " + name;
            _show.Episodes.ForEach(x => x.ShowName = name);
            return this;
        }

        public ShowBuilder AddMissingEpisodes(int count)
        {
            _show.MissingEpisodesCount = count;
            return this;
        }

        public ShowBuilder AddCommunityRating(float? value)
        {
            _show.CommunityRating = value;
            return this;
        }

        public ShowBuilder AddOfficialRating(string value)
        {
            _show.OfficialRating = value;
            return this;
        }

        public ShowBuilder AddPremiereDate(DateTime date)
        {
            _show.PremiereDate = date;
            return this;
        }

        public ShowBuilder AddEpisode(Episode episode)
        {
            _show.Episodes.Add(episode);
            return this;
        }

        public ShowBuilder ClearEpisodes()
        {
            _show.Episodes = new List<Episode>();
            return this;
        }

        public ShowBuilder AddCreateDate(DateTime date)
        {
            _show.DateCreated = date;
            return this;
        }

        public ShowBuilder AddGenre(params string[] genres)
        {
            _show.Genres = genres;
            return this;
        }

        public ShowBuilder SetContinuing()
        {
            _show.Status = "Continuing";
            return this;
        }

        public ShowBuilder AddActor(string id)
        {
            var list = _show.People.ToList();
            list.Add(new ExtraPerson {Id = id, Name = "Gimli", Type = PersonType.Actor});
            _show.People = list.ToArray();
            return this;
        }

        public Show Build()
        {
            return _show;
        }
    }
}
