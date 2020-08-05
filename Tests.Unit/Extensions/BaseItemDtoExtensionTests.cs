using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Net;
using FluentAssertions;
using Xunit;

namespace Tests.Unit.Extensions
{
    public class BaseItemDtoExtensionTests
    {
        [Fact]
        public void MapStreams_Should_Map_All_Streams()
        {
            var dto = new BaseItemDto()
            {
                MediaStreams = new[]
                {
                    new BaseMediaStream
                    {
                        BitRate = 1000,
                        ChannelLayout = "channel",
                        Channels = 2,
                        Codec = "codec",
                        Language = "nl",
                        SampleRate = 1000,
                        Type = MediaStreamType.Audio
                    },
                    new BaseMediaStream
                    {
                        Codec = "codec",
                        DisplayTitle = "title",
                        IsDefault = true,
                        Language = "nl",
                        Type = MediaStreamType.Subtitle,
                    },
                    new BaseMediaStream
                    {
                        Language = "nl",
                        Type = MediaStreamType.Video,
                        BitRate = 1000,
                        AspectRatio = "ratio",
                        AverageFrameRate = 0.1f,
                        Channels = 2,
                        Height = 100,
                        Width = 100
                    }
                }
            };

            var video = new Video();
            dto.MapStreams(video);

            video.AudioStreams.Count.Should().Be(1);
            video.VideoStreams.Count.Should().Be(1);
            video.SubtitleStreams.Count.Should().Be(1);
        }

        [Fact]
        public void MapStreams_Should_Not_Map_Anything()
        {
            var dto = new BaseItemDto();
            var video = new Video();
            dto.MapStreams(video);

            video.AudioStreams.Count.Should().Be(0);
            video.VideoStreams.Count.Should().Be(0);
            video.SubtitleStreams.Count.Should().Be(0);
        }

        [Fact]
        public void MapPeople_Should_Map_People()
        {
            var extra = new Extra();
            var dto = new BaseItemDto
            {
                People = new[]
                {
                    new BaseItemPerson
                    {
                        Name = "name",
                        Id = "123",
                        Type = PersonType.Actor
                    }
                }
            };

            dto.MapPeople(extra);
            extra.People.Length.Should().Be(1);
        }

        [Fact]
        public void MapPeople_Should_Not_Map_People()
        {
            var dto = new BaseItemDto();
            var video = new Video();
            dto.MapPeople(video);

            video.People.Length.Should().Be(0);
        }

        [Fact]
        public void MapImageTags_Should_Not_Map_Images()
        {
            var dto = new BaseItemDto();
            var video = new Video();
            dto.MapImageTags(video);

            video.Logo.Should().BeNull();
            video.Banner.Should().BeNull();
            video.Thumb.Should().BeNull();
            video.Banner.Should().BeNull();
        }

        [Fact]
        public void MapImageTags_Should_Map_To_Empty_Images()
        {
            var dto = new BaseItemDto { ImageTags = new Dictionary<ImageType, string>() };
            var video = new Video();
            dto.MapImageTags(video);

            video.Logo.Should().BeEmpty();
            video.Banner.Should().BeEmpty();
            video.Thumb.Should().BeEmpty();
            video.Banner.Should().BeEmpty();
        }

        [Fact]
        public void MapImageTags_Should_Map_Images()
        {
            var dto = new BaseItemDto { ImageTags = new Dictionary<ImageType, string>()
            {
                { ImageType.Banner, "banner" },
                { ImageType.Logo, "logo" },
                { ImageType.Thumb, "thumb" },
                { ImageType.Primary, "primary" }
            } };
            var video = new Video();
            dto.MapImageTags(video);

            video.Logo.Should().Be("logo");
            video.Banner.Should().Be("banner");
            video.Thumb.Should().Be("thumb");
            video.Primary.Should().Be("primary");
        }

        [Fact]
        public void MapProviderIds_Should_Not_Map_ProviderIds()
        {
            var dto = new BaseItemDto();
            var video = new Video();
            dto.MapProviderIds(video);

            video.TMDB.Should().BeNull();
            video.TVDB.Should().BeNull();
            video.IMDB.Should().BeNull();
        }

        [Fact]
        public void MapProviderIds_Should_Map_To_Empty_ProviderIds()
        {
            var dto = new BaseItemDto() { ProviderIds = new Dictionary<string, string>() };
            var video = new Video();
            dto.MapProviderIds(video);

            video.TMDB.Should().BeEmpty();
            video.TVDB.Should().BeEmpty();
            video.IMDB.Should().BeEmpty();
        }

        [Fact]
        public void MapProviderIds_Should_Map_ProviderIds()
        {
            var dto = new BaseItemDto { ProviderIds = new Dictionary<string, string>()
            {
                { "Tmdb", "1234" },
                { "Tvdb", "2345" },
                { "Imdb", "3456" }
            } };
            var video = new Video();
            dto.MapProviderIds(video);

            video.TMDB.Should().Be("1234");
            video.TVDB.Should().Be("2345");
            video.IMDB.Should().Be("3456");
        }

        [Fact]
        public void MapMediaSources_Should_Not_Map_MediaSources()
        {
            var dto = new BaseItemDto();
            var video = new Video();
            dto.MapMediaSources(video);

            video.MediaSources.Count.Should().Be(0);
        }

        [Fact]
        public void MapMediaSources_Should_Map_To_Empty_MediaSources()
        {
            var dto = new BaseItemDto { MediaSources = new BaseMediaSourceInfo[0] };
            var video = new Video();
            dto.MapMediaSources(video);

            video.MediaSources.Count.Should().Be(0);
        }

        [Fact]
        public void MapMediaSources_Should_Map_MediaSources()
        {
            var dto = new BaseItemDto
            {
                MediaSources = new []
                {
                    new BaseMediaSourceInfo(), 
                    new BaseMediaSourceInfo()
                }
            };
            var video = new Video();
            dto.MapMediaSources(video);

            video.MediaSources.Count.Should().Be(2);
        }
    }
}
