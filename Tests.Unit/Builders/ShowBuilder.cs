using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Clients.Base.Models;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;

namespace Tests.Unit.Builders
{
    public class ShowBuilder
    {
        private readonly Show _show;

        public ShowBuilder(string id, string libraryId)
        {
            _show = new Show
            {
                Id = id,
                Path = "Path/To/Show",
                Banner = "banner.png",
                CollectionId = libraryId,
                CommunityRating = 1.7f,
                CumulativeRunTimeTicks = 19400287400000,
                DateCreated = new DateTimeOffset(2001, 01, 01, 0, 0, 0, new TimeSpan(0)),
                IMDB = "12345",
                Logo = "logo.jpg",
                Name = "Chuck",
                OfficialRating = "R",
                ParentId = libraryId,
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
                    new EpisodeBuilder(Guid.NewGuid().ToString(), id, "1").Build(),
                    new EpisodeBuilder(Guid.NewGuid().ToString(), id, "1").WithIndexNumber(1).Build(),
                },
                Seasons = new List<Season>
                {
                    new SeasonBuilder(Guid.NewGuid().ToString(), id).Build()
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

        public ShowBuilder AddPerson(ExtraPerson person)
        {
            var list = _show.People.ToList();
            list.Add(person);
            _show.People = list.ToArray();
            return this;
        }

        public ShowBuilder AddMissingEpisodes(int count, int seasonIndex)
        {
            var season = _show.Seasons[seasonIndex];
            for (var i = 0; i < count; i++)
            {
                _show.Episodes.Add(new EpisodeBuilder(Guid.NewGuid().ToString(), _show.Id, season.Id)
                    .WithIndexNumber(i)
                    .WithLocationType(LocationType.Virtual)
                    .Build());
            }

            return this;
        }

        public ShowBuilder AddSeason(int indexNumber, int extraEpisodes)
        {
            var seasonId = Guid.NewGuid().ToString();
            _show.Seasons.Add(new SeasonBuilder(seasonId, _show.Id).WithIndexNumber(indexNumber).Build());

            for (var i = 0; i < extraEpisodes; i++)
            {
                _show.Episodes.Add(new EpisodeBuilder(Guid.NewGuid().ToString(), _show.Id, seasonId).Build());
            }

            return this;
        }
        
        public ShowBuilder AddFailedSync(bool state)
        {
            _show.TvdbFailed = state;
            return this;
        }

        public ShowBuilder AddTvdbSynced(bool state)
        {
            _show.TvdbSynced = state;
            return this;
        }

        public ShowBuilder AddUpdateState(DateTime updated)
        {
            _show.LastUpdated = updated;
            return this;
        }

        public Show Build()
        {
            return _show;
        }

        public BaseItemDto BuildBaseItemDto()
        {
            return new BaseItemDto
            {
                Id = _show.Id,
                CommunityRating = _show.CommunityRating,
                DateCreated = _show.DateCreated,
                ParentId = _show.ParentId,
                Path = _show.Path,
                SortName = _show.SortName,
                RunTimeTicks = _show.RunTimeTicks,
                OfficialRating = _show.OfficialRating,
                PremiereDate = _show.PremiereDate,
                ProductionYear = _show.ProductionYear,
                ImageTags = new Dictionary<ImageType, string>
                {
                    {ImageType.Primary, _show.Primary},
                    {ImageType.Thumb, _show.Primary},
                    {ImageType.Logo, _show.Primary},
                    {ImageType.Banner, _show.Primary}
                },
                ProviderIds = new Dictionary<string, string>
                {
                    {"Imdb", _show.IMDB},
                    {"Tmdb", _show.TMDB},
                    {"Tvdb", _show.TVDB}
                },
                Genres = _show.Genres,
                People = _show.People.Select(x => new BaseItemPerson
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = x.Type
                }).ToArray()
            };
        }
    }
}
