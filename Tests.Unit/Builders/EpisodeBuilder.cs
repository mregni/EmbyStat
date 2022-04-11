using System;
using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Entities.Streams;
using MoreLinq.Extensions;

namespace Tests.Unit.Builders;

public class EpisodeBuilder
{
    private readonly Episode _episode;

    public EpisodeBuilder(string id, string seasonId)
    {
        _episode = new Episode
        {
            Id = id,
            Path = "path/to/episode",
            Name = "EpisodeName",
            SeasonId = seasonId,
            CommunityRating = 0.7M,
            Container = "mkv",
            DateCreated = new DateTime(2001, 1, 1, 0, 0, 0),
            IndexNumber = 0,
            IndexNumberEnd = null,
            ProductionYear = 2001,
            PremiereDate = new DateTime(2001, 1, 1, 0, 0, 0),
            RunTimeTicks = 10000000,
            SortName = "0001 - EpisodeName",
            Primary = "123987987987",
            Thumb = "1234",
            Banner = "3434",
            Logo = "logo",
            OfficialRating = "TV",
            IMDB = "12345",
            TMDB = 123456,
            TVDB = "23456",
            AudioStreams = new List<AudioStream>{
                new() { Id = "1", BitRate = 320000, ChannelLayout = "5.1", Channels = 6, Codec = "aac", Language = "en", SampleRate = 48000 }
            },
            SubtitleStreams = new List<SubtitleStream>{
                new() { Codec = "srt", DisplayTitle = "Dut", Id = "123", Language = "dut", IsDefault = true }
            },
            VideoStreams = new List<VideoStream>
            {
                new() { AspectRatio = "16:9", AverageFrameRate = 24, BitRate = 1239588, Id = "1234", Language = "eng", Channels = 6, Height = 1080, Width = 1920 }
            },
            MediaSources = new List<MediaSource>
            {
                new() { Id = "12345", Path = "path/to/video", BitRate = 123456, Container = "mkv", Protocol = "File", RunTimeTicks = 1230498908, SizeInMb = 101 }
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