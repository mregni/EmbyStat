using System;
using System.Collections.Generic;
using System.Text;
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
            var extra = new Video();
            dto.MapPeople(extra);

            extra.People.Length.Should().Be(0);
        }

        [Fact]
        public void MapImageTags_Should_Not_Map_Images()
        {
            var dto = new BaseItemDto();
            var extra = new Video();
            dto.MapImageTags(extra);

            extra.Logo.Should().BeNull();
            extra.Banner.Should().BeNull();
            extra.Thumb.Should().BeNull();
            extra.Banner.Should().BeNull();
        }

        [Fact]
        public void MapImageTags_Should_Map_To_Empty_Images()
        {
            var dto = new BaseItemDto { ImageTags = new Dictionary<ImageType, string>() };
            var extra = new Video();
            dto.MapImageTags(extra);

            extra.Logo.Should().BeEmpty();
            extra.Banner.Should().BeEmpty();
            extra.Thumb.Should().BeEmpty();
            extra.Banner.Should().BeEmpty();
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
            var extra = new Video();
            dto.MapImageTags(extra);

            extra.Logo.Should().Be("logo");
            extra.Banner.Should().Be("banner");
            extra.Thumb.Should().Be("thumb");
            extra.Primary.Should().Be("primary");
        }

        [Fact]
        public void MapProviderIds_Should_Not_Map_ProviderIds()
        {
            var dto = new BaseItemDto();
            var extra = new Video();
            dto.MapProviderIds(extra);

            extra.TMDB.Should().BeNull();
            extra.TVDB.Should().BeNull();
            extra.IMDB.Should().BeNull();
        }

        [Fact]
        public void MapProviderIds_Should_Map_To_Empty_ProviderIds()
        {
            var dto = new BaseItemDto() { ProviderIds = new Dictionary<string, string>() };
            var extra = new Video();
            dto.MapProviderIds(extra);

            extra.TMDB.Should().BeEmpty();
            extra.TVDB.Should().BeEmpty();
            extra.IMDB.Should().BeEmpty();
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
            var extra = new Video();
            dto.MapProviderIds(extra);

            extra.TMDB.Should().Be("1234");
            extra.TVDB.Should().Be("2345");
            extra.IMDB.Should().Be("3456");
        }
    }
}
