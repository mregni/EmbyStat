using System;
using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;

namespace Tests.Unit.Builders
{
    public class EpisodeBuilder
    {
        private readonly Episode _episode;

        public EpisodeBuilder(int id, int showId, string seasonId)
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
                DateCreated = new DateTimeOffset(2001, 1, 1, 0, 0, 0, new TimeSpan(0)),
                IndexNumber = 0,
                IndexNumberEnd = null,
                MediaType = "Episode",
                ProductionYear = 2001,
                PremiereDate = new DateTimeOffset(2001, 1, 1, 0, 0, 0, new TimeSpan(0)),
                RunTimeTicks = 10000000,
                SortName = "0001 - EpisodeName",
                ShowName = "Chuck",
                Primary = "123987987987",
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

        public Episode Build()
        {
            return _episode;
        }
    }
}
