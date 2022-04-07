﻿using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Entities.Streams;
using EmbyStat.Common.Models.Net;

namespace Tests.Unit.Builders
{
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

        public MovieBuilder AddLogo(string image)
        {
            _movie.Logo = image;
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

        public MovieBuilder ReplacePersons(MediaPerson person)
        {
            _movie.People = new[] {person};
            return this;
        }

        public MovieBuilder AddAudioStream(AudioStream stream)
        {
            _movie.AudioStreams.Add(stream);
            return this;
        }

        public MovieBuilder AddSubtitleStream(SubtitleStream stream)
        {
            _movie.SubtitleStreams.Add(stream);
            return this;
        }

        public MovieBuilder AddVideoStream(VideoStream stream)
        {
            _movie.VideoStreams.Add(stream);
            return this;
        }

        public MovieBuilder AddContainer(string container)
        {
            _movie.Container = container;
            return this;
        }

        public MovieBuilder ReplaceSubtitleStream(SubtitleStream stream)
        {
            _movie.SubtitleStreams = new List<SubtitleStream> {stream};
            return this;
        }

        public MovieBuilder EmptySubtitleStreams()
        {
            _movie.SubtitleStreams = null;
            return this;
        }

        public MovieBuilder ReplaceMediaSources(MediaSource source)
        {
            _movie.MediaSources = new List<MediaSource> {source};
            return this;
        }

        public MovieBuilder ReplaceVideoStreams(VideoStream stream)
        {
            _movie.VideoStreams = new List<VideoStream> {stream};
            return this;
        }

        public Movie Build()
        {
            return _movie;
        }

        public BaseItemDto BuildBaseItemDto()
        {
            return new BaseItemDto
            {
                Id = _movie.Id,
                CommunityRating = _movie.CommunityRating,
                Container = _movie.Container,
                DateCreated = _movie.DateCreated,
                OriginalTitle = _movie.OriginalTitle,
                Path = _movie.Path,
                SortName = _movie.SortName,
                MediaSources = _movie.MediaSources.Select(x => new BaseMediaSourceInfo
                {
                    Id = x.Id,
                    Path = x.Path,
                    Bitrate = x.BitRate,
                    Container = x.Container,
                    Protocol = MediaProtocol.File,
                    RunTimeTicks = x.RunTimeTicks,
                    Size = 1000
                }).ToArray(),
                RunTimeTicks = _movie.RunTimeTicks,
                OfficialRating = _movie.OfficialRating,
                PremiereDate = _movie.PremiereDate,
                ProductionYear = _movie.ProductionYear,
                Video3DFormat = _movie.Video3DFormat,
                ImageTags = new Dictionary<ImageType, string>
                {
                    {ImageType.Primary, _movie.Primary},
                    {ImageType.Thumb, _movie.Thumb},
                    {ImageType.Logo, _movie.Logo},
                    {ImageType.Banner, _movie.Banner}
                },
                ProviderIds = new Dictionary<string, string>
                {
                    {"Imdb", _movie.IMDB},
                    {"Tmdb", _movie.TMDB.ToString()},
                    {"Tvdb", _movie.TVDB}
                },
                MediaStreams = _movie.AudioStreams.Select(x => new BaseMediaStream
                {
                    BitRate = x.BitRate,
                    ChannelLayout = x.ChannelLayout,
                    Channels = x.Channels,
                    Codec = x.Codec,
                    Language = x.Language,
                    SampleRate = x.SampleRate,
                    Type = MediaStreamType.Audio
                }).Union(_movie.SubtitleStreams.Select(x => new BaseMediaStream
                {
                    Language = x.Language,
                    Codec = x.Codec,
                    DisplayTitle = x.DisplayTitle,
                    IsDefault = x.IsDefault,
                    Type = MediaStreamType.Subtitle
                })).Union(_movie.VideoStreams.Select(x => new BaseMediaStream
                {
                    Language = x.Language,
                    BitRate = x.BitRate,
                    AspectRatio = x.AspectRatio,
                    AverageFrameRate = x.AverageFrameRate,
                    Channels = x.Channels,
                    Height = x.Height,
                    Width = x.Width,
                    Type = MediaStreamType.Video
                })).ToArray(),
                Genres = _movie.Genres.Select(x => x.Id).ToArray(),
                People = _movie.People.Select(x => new BaseItemPerson
                {
                    Id = x.Id.ToString(),
                    Name = x.Person.Name,
                    Type = x.Type
                }).ToArray()
            };
        }
    }
}