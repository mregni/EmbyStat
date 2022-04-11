using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Entities.Streams;

namespace Tests.Unit.Builders;

public class MovieBuilder
{
    private readonly Movie _movie;

    public MovieBuilder(string id)
    {
        _movie = new Movie
        {
            CommunityRating = 1.7M,
            Id = id,
            Name = "The lord of the rings",
            PremiereDate = new DateTime(2002, 1, 1),
            DateCreated = new DateTime(2018, 1, 1),
            OfficialRating = "R",
            RunTimeTicks = 120000000000,
            Primary = "primaryImage",
            Logo = "logoImage",
            Banner = "bannerImage",
            TMDB = 1000,
            IMDB = "0001",
            TVDB = "0003",
            Thumb = "thumbImage",
            Video3DFormat = Video3DFormat.None,
            Genres = new Genre[] {new() {Id = "1", Name = "Action"}},
            People = new MediaPerson[]
            {
                new()
                {
                    Id = 1,
                    Type = PersonType.Actor,
                    Person = new Person {Name = "Gimli"}
                }
            },
            MediaSources = new List<MediaSource> {new() {SizeInMb = 2000}},
            Container = "avi",
            ProductionYear = 2000,
            SubtitleStreams = new List<SubtitleStream>
            {
                new SubtitleStreamBuilder("en").Build()
            },
            VideoStreams = new List<VideoStream>
            {
                new VideoStreamBuilder().Build()
            }
        };
    }

    public MovieBuilder AddSortName(string name)
    {
        _movie.SortName = name;
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

    public MovieBuilder AddCommunityRating(decimal rating)
    {
        _movie.CommunityRating = rating;
        return this;
    }

    public MovieBuilder AddPremiereDate(DateTime? date)
    {
        _movie.PremiereDate = date;
        return this;
    }

    public MovieBuilder AddGenres(params Genre[] genres)
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

    public MovieBuilder AddPerson(MediaPerson person)
    {
        var list = _movie.People.ToList();
        list.Add(person);
        _movie.People = list.ToArray();
        return this;
    }

    public Movie Build()
    {
        return _movie;
    }
}