using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Shows;
using MoreLinq.Extensions;

namespace Tests.Unit.Builders;

public class ShowBuilder
{
    private readonly Show _show;

    public ShowBuilder(string id)
    {
        _show = new Show
        {
            Id = id,
            Path = "Path/To/Show",
            Banner = "banner.png",
            CommunityRating = 1.7M,
            CumulativeRunTimeTicks = 19400287400000,
            DateCreated = new DateTime(2001, 01, 01, 0, 0, 0),
            IMDB = "12345",
            Logo = "logo.jpg",
            Name = "Chuck",
            OfficialRating = "R",
            Primary = "primary.jpg",
            ProductionYear = 2001,
            RunTimeTicks = 1200000000,
            SortName = "Chuck",
            Status = "Ended",
            TMDB = 12345,
            SizeInMb = 303,
            TVDB = "12345",
            Thumb = "thumb.jpg",
            ExternalSynced = false,
            PremiereDate = new DateTime(2001, 01, 01, 0, 0, 0),
            People = new MediaPerson[]
            {
                new()
                {
                    Id = 1,
                    Type = PersonType.Actor,
                    Person = new Person {Name = "Gimli"}
                }
            },
            Genres = new Genre[] {new() {Id = "1", Name = "Action"}},
            Seasons = new List<Season>
            {
                new SeasonBuilder(Guid.NewGuid().ToString(), id).WithIndexNumber(0).Build(),
                new SeasonBuilder(Guid.NewGuid().ToString(), id).Build()
            }
        };
    }

    public ShowBuilder AddName(string name)
    {
        _show.Name = name;
        _show.SortName = "0001 - " + name;
        return this;
    }

    public ShowBuilder AddCommunityRating(decimal? value)
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

    public ShowBuilder AddCreateDate(DateTime date)
    {
        _show.DateCreated = date;
        return this;
    }

    public ShowBuilder AddGenre(params string[] genres)
    {
        _show.Genres = genres.Select(x => new Genre {Name = x}).ToList();
        return this;
    }

    public ShowBuilder SetContinuing()
    {
        _show.Status = "Continuing";
        return this;
    }

    public ShowBuilder AddEpisode(Episode episode)
    {
        var season = _show.Seasons.FirstOrDefault(x => x.Id == episode.SeasonId);
        if (season == null)
        {
            _show.Seasons.Add(new SeasonBuilder(episode.SeasonId, _show.Id).Build());
            season = _show.Seasons.First(x => x.Id == episode.SeasonId);
        }

        season.Episodes.Add(episode);
        return this;
    }

    public ShowBuilder AddMissingEpisodes(int count, int seasonIndex)
    {
        var season = _show.Seasons.ToArray()[seasonIndex];
        for (var i = 0; i < count; i++)
        {
            season.Episodes.Add(new EpisodeBuilder(Guid.NewGuid().ToString(), season.Id)
                .WithIndexNumber(i)
                .WithLocationType(LocationType.Virtual)
                .Build());
        }

        return this;
    }

    public ShowBuilder AddActor(string id)
    {
        var list = _show.People.ToList();
        list.Add(new MediaPerson {PersonId = id});
        _show.People = list;
        return this;
    }
        
    public ShowBuilder ClearEpisodes()
    {
        _show.Seasons.ForEach(x => x.Episodes = new List<Episode>());
        return this;
    }

    public Show Build()
    {
        return _show;
    }
}