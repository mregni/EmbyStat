using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Net;

namespace Tests.Unit.Builders
{
    public class EpisodeBuilder
    {
        private readonly Episode _episode;

        public EpisodeBuilder(string id, string showId, string seasonId)
        {
            _episode = new Episode
            {
                Id = id,
                Path = "path/to/episode",
                ShowId = showId,
                Name = "EpisodeName",
                ParentId = seasonId,
                CommunityRating = 0.7f,
                Container = "mkv",
                DateCreated = new DateTime(2001, 1, 1, 0, 0, 0),
                IndexNumber = 0,
                IndexNumberEnd = null,
                MediaType = "Episode",
                ProductionYear = 2001,
                PremiereDate = new DateTime(2001, 1, 1, 0, 0, 0),
                RunTimeTicks = 10000000,
                SortName = "0001 - EpisodeName",
                ShowName = "Chuck",
                Primary = "123987987987",
                Thumb = "1234",
                Banner = "3434",
                Logo = "logo",
                OfficialRating = "TV",
                IMDB = "12345",
                TMDB = "123456",
                TVDB = "23456",
                AudioStreams = new List<AudioStream>{
                    new AudioStream{ Id = "1", BitRate = 320000, ChannelLayout = "5.1", Channels = 6, Codec = "aac", Language = "en", SampleRate = 48000 }
                },
                SubtitleStreams = new List<SubtitleStream>{
                    new SubtitleStream{ Codec = "srt", DisplayTitle = "Dut", Id = "123", Language = "dut", IsDefault = true }
                },
                VideoStreams = new List<VideoStream>
                {
                    new VideoStream{ AspectRatio = "16:9", AverageFrameRate = 24, BitRate = 1239588, Id = "1234", Language = "eng", Channels = 6, Height = 1080, Width = 1920 }
                },
                MediaSources = new List<MediaSource>
                {
                    new MediaSource { Id = "12345", Path = "path/to/video", BitRate = 123456, Container = "mkv", Protocol = "File", RunTimeTicks = 1230498908, SizeInMb = 101 }
                }
            };
        }

        public EpisodeBuilder WithLocationType(LocationType type)
        {
            _episode.LocationType = type;
            if (type == LocationType.Virtual)
            {
                _episode.MediaSources.ForEach(x => x.SizeInMb = 0);
            }
            return this;
        }

        public EpisodeBuilder WithIndexNumber(int index)
        {
            _episode.IndexNumber = index;
            return this;
        }

        public EpisodeBuilder WithIndexNumberEnd(int index)
        {
            _episode.IndexNumberEnd = index;
            return this;
        }

        public Episode Build()
        {
            return _episode;
        }

        public BaseItemDto BuildBaseItemDto()
        {
            return new BaseItemDto
            {
                Id = _episode.Id,
                Name = _episode.Name,
                CommunityRating = _episode.CommunityRating,
                IndexNumber = _episode.IndexNumber,
                IndexNumberEnd = _episode.IndexNumberEnd,
                Container = _episode.Container,
                DateCreated = _episode.DateCreated,
                ParentId = _episode.ParentId,
                Path = _episode.Path,
                SortName = _episode.SortName,
                MediaSources = _episode.MediaSources.Select(x => new BaseMediaSourceInfo
                {
                    Id = x.Id,
                    Path = x.Path,
                    Bitrate = x.BitRate,
                    Container = x.Container,
                    Protocol = MediaProtocol.File,
                    RunTimeTicks = x.RunTimeTicks,
                    Size = 1000
                }).ToArray(),
                RunTimeTicks = _episode.RunTimeTicks,
                Type = _episode.MediaType,
                PremiereDate = _episode.PremiereDate,
                ProductionYear = _episode.ProductionYear,
                Video3DFormat = _episode.Video3DFormat,
                ImageTags = new Dictionary<ImageType, string>
                {
                    {ImageType.Primary, _episode.Primary},
                    {ImageType.Thumb, _episode.Thumb},
                    {ImageType.Logo, _episode.Logo},
                    {ImageType.Banner, _episode.Banner}
                },
                ProviderIds = new Dictionary<string, string>
                {
                    {"Imdb", _episode.IMDB},
                    {"Tmdb", _episode.TMDB},
                    {"Tvdb", _episode.TVDB}
                },
                MediaStreams = _episode.AudioStreams.Select(x => new BaseMediaStream
                {
                    BitRate = x.BitRate,
                    ChannelLayout = x.ChannelLayout,
                    Channels = x.Channels,
                    Codec = x.Codec,
                    Language = x.Language,
                    SampleRate = x.SampleRate,
                    Type = MediaStreamType.Audio
                }).Union(_episode.SubtitleStreams.Select(x => new BaseMediaStream
                {
                    Language = x.Language,
                    Codec = x.Codec,
                    DisplayTitle = x.DisplayTitle,
                    IsDefault = x.IsDefault,
                    Type = MediaStreamType.Subtitle
                })).Union(_episode.VideoStreams.Select(x => new BaseMediaStream
                {
                    Language = x.Language,
                    BitRate = x.BitRate,
                    AspectRatio = x.AspectRatio,
                    AverageFrameRate = x.AverageFrameRate,
                    Channels = x.Channels,
                    Height = x.Height,
                    Width = x.Width,
                    Type = MediaStreamType.Video
                })).ToArray()
            };
        }
    }
}
