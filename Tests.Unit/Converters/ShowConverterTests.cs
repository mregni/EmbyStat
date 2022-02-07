using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Clients.Base.Converters;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Net;
using EmbyStat.Common.Models.Show;
using EmbyStat.Logging;
using FluentAssertions;
using Tests.Unit.Builders;
using Xunit;

namespace Tests.Unit.Converters
{

    public class ShowConverterTests
    {
        [Fact]
        public void ConvertToShow_Should_Return_A_Show()
        {
            var logger = LogFactory.CreateLoggerForType(typeof(ShowConverterTests), string.Empty);
            var data = new BaseItemDto
            {
                Id = "1",
                ImageTags = new Dictionary<ImageType, string>
                {
                    {ImageType.Primary, "primary"}, {ImageType.Thumb, "thumb"}, {ImageType.Logo, "logo"},
                    {ImageType.Banner, "banner"}
                },
                Name = "2 broke girls",
                ParentId = "123",
                Path = "c:\\",
                CommunityRating = 1.2f,
                DateCreated = new DateTime(2000, 1, 1, 1, 1, 1),
                ProviderIds = new Dictionary<string, string>
                {
                    { "Imdb", "1234" },
                    { "Tmdb", "1234" },
                    { "Tvdb", "1234" }
                },
                OfficialRating = "16TV",
                PremiereDate = new DateTime(2000, 1, 1, 1, 1, 1),
                ProductionYear = 2001,
                RunTimeTicks = 1000,
                SortName = "2 broke girls",
                Status = "Ended",
                Genres = new []{ "Action" },
                People = new []
                {
                    new BaseItemPerson()
                    {
                        Id = "123",
                        Name = "Gimli",
                        Type = PersonType.Actor
                    } 
                }
            };

            var show = data.ConvertToShow("123", logger);
            show.Id.Should().Be(data.Id);
            show.CollectionId.Should().Be("123");
            show.Primary.Should().Be(data.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value);
            show.Thumb.Should().Be(data.ImageTags.FirstOrDefault(y => y.Key == ImageType.Thumb).Value);
            show.Logo.Should().Be(data.ImageTags.FirstOrDefault(y => y.Key == ImageType.Logo).Value);
            show.Banner.Should().Be(data.ImageTags.FirstOrDefault(y => y.Key == ImageType.Banner).Value);
            show.Name.Should().Be(data.Name);
            show.ParentId.Should().Be(data.ParentId);
            show.Path.Should().Be(data.Path);
            show.CommunityRating.Should().Be(data.CommunityRating);
            show.DateCreated.Should().Be(data.DateCreated);
            show.OfficialRating.Should().Be(data.OfficialRating);
            show.IMDB.Should().Be(data.ProviderIds.FirstOrDefault(y => y.Key == "Imdb").Value);
            show.TMDB.Should().Be(int.Parse(data.ProviderIds.FirstOrDefault(y => y.Key == "Tmdb").Value));
            show.TVDB.Should().Be(data.ProviderIds.FirstOrDefault(y => y.Key == "Tvdb").Value);
            show.PremiereDate.Should().Be(data.PremiereDate);
            show.ProductionYear.Should().Be(data.ProductionYear);
            show.RunTimeTicks.Should().Be(data.RunTimeTicks);
            show.SortName.Should().Be(data.SortName);
            show.Status.Should().Be(data.Status);
            show.Genres.Length.Should().Be(1);
            show.People.Length.Should().Be(1);
            show.Seasons.Count.Should().Be(0);
            show.Episodes.Count.Should().Be(0);
        }

        [Fact]
        public void ConvertToSeason_Should_Return_Season()
        {
            var logger = LogFactory.CreateLoggerForType(typeof(ShowConverterTests), string.Empty);
            var data = new BaseItemDto
            {
                Id = "123",
                Name = "Season 01",
                ParentId = "12",
                Path = "C:\\",
                DateCreated = new DateTime(2000, 1, 1, 1, 1, 1),
                IndexNumber = 1,
                IndexNumberEnd = null,
                PremiereDate = new DateTime(2000, 1, 1, 1, 1, 1),
                ProductionYear = 2001,
                SortName = "season 01",
                ImageTags = new Dictionary<ImageType, string>
                {
                    {ImageType.Primary, "primary"}, {ImageType.Thumb, "thumb"}, {ImageType.Logo, "logo"},
                    {ImageType.Banner, "banner"}
                }
            };

            var season = data.ConvertToSeason(logger);
            season.Id.Should().Be(data.Id);
            season.ParentId.Should().Be(data.ParentId);
            season.Name.Should().Be(data.Name);
            season.Path.Should().Be(data.Path);
            season.DateCreated.Should().Be(data.DateCreated);
            season.IndexNumber.Should().Be(data.IndexNumber);
            season.IndexNumberEnd.Should().Be(data.IndexNumberEnd);
            season.PremiereDate.Should().Be(data.PremiereDate);
            season.ProductionYear.Should().Be(data.ProductionYear);
            season.SortName.Should().Be(data.SortName);
            season.Primary.Should().Be(data.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value);
            season.Thumb.Should().Be(data.ImageTags.FirstOrDefault(y => y.Key == ImageType.Thumb).Value);
            season.Logo.Should().Be(data.ImageTags.FirstOrDefault(y => y.Key == ImageType.Logo).Value);
            season.Banner.Should().Be(data.ImageTags.FirstOrDefault(y => y.Key == ImageType.Banner).Value);
            season.LocationType.Should().Be(LocationType.Disk);
        }

        [Fact]
        public void ConvertToSeason_Should_Return_Season_From_Int()
        {
            var seasonIndex = 1;
            var show = new Show
            {
                Id = "1",
            };

            var season = seasonIndex.ConvertToVirtualSeason(show);
            season.Id.Should().NotBeNullOrWhiteSpace();
            season.Name.Should().Be($"Season {seasonIndex}");
            season.ParentId.Should().Be(show.Id);
            season.Path.Should().Be(string.Empty);
            season.DateCreated.Should().BeNull();
            season.IndexNumber.Should().Be(seasonIndex);
            season.IndexNumberEnd.Should().Be(seasonIndex);
            season.PremiereDate.Should().BeNull();
            season.ProductionYear.Should().BeNull();
            season.SortName.Should().Be("0001");
            season.LocationType.Should().Be(LocationType.Virtual);
        }

        [Fact]
        public void ConvertToSeason_Should_Return_Special_Season_From_Int()
        {
            var seasonIndex = 0;
            var show = new Show
            {
                Id = "1",
            };

            var season = seasonIndex.ConvertToVirtualSeason(show);
            season.Id.Should().NotBeNullOrWhiteSpace();
            season.Name.Should().Be($"Special");
        }

        [Fact]
        public void ConvertToEpisode_Should_Convert_Virtual_Episode()
        {
            var show = new ShowBuilder("1", "1").Build();
            var showEpisode = show.Episodes.First();
            var showSeason = show.Seasons.First();

            var virtualEpisode = new VirtualEpisode(showEpisode);
            var episode = virtualEpisode.ConvertToVirtualEpisode(show, showSeason);

            episode.ShowId.Should().Be(show.Id);
            episode.ShowName.Should().Be(show.Name);
            episode.Name.Should().Be(showEpisode.Name);
            episode.LocationType.Should().Be(LocationType.Virtual);
            episode.IndexNumber.Should().Be(showEpisode.IndexNumber);
            episode.ParentId.Should().Be(showSeason.Id);
            // ReSharper disable once PossibleInvalidOperationException
            episode.PremiereDate.Should().Be(showEpisode.PremiereDate);
        }

        [Fact]
        public void ConvertToEpisode_Should_Return_An_Episode()
        {
            var baseItem = new EpisodeBuilder("123", "456", "1").BuildBaseItemDto();
            var logger = LogFactory.CreateLoggerForType(typeof(ShowConverterTests), string.Empty);
            var episode = baseItem.ConvertToEpisode("456", logger);

            episode.Should().NotBeNull();
            episode.Id.Should().NotBeNullOrWhiteSpace();
            episode.Name.Should().Be(baseItem.Name);
            episode.ParentId.Should().Be(baseItem.ParentId);
            episode.DateCreated.Should().Be(baseItem.DateCreated);
            episode.Path.Should().Be(baseItem.Path);
            episode.SortName.Should().Be(baseItem.SortName);
            episode.RunTimeTicks.Should().Be(baseItem.RunTimeTicks);
            episode.Container.Should().Be(baseItem.Container);
            episode.CommunityRating.Should().Be(baseItem.CommunityRating);
            episode.MediaType.Should().Be(baseItem.Type);
            episode.PremiereDate.Should().Be(baseItem.PremiereDate);
            episode.ProductionYear.Should().Be(baseItem.ProductionYear);
            episode.Video3DFormat.Should().Be(baseItem.Video3DFormat);
            episode.Primary.Should().Be(baseItem.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value);
            episode.Thumb.Should().Be(baseItem.ImageTags.FirstOrDefault(y => y.Key == ImageType.Thumb).Value);
            episode.Logo.Should().Be(baseItem.ImageTags.FirstOrDefault(y => y.Key == ImageType.Logo).Value);
            episode.Banner.Should().Be(baseItem.ImageTags.FirstOrDefault(y => y.Key == ImageType.Banner).Value);
            episode.IMDB.Should().Be(baseItem.ProviderIds.FirstOrDefault(y => y.Key == "Imdb").Value);
            episode.TMDB.Should().Be(int.Parse(baseItem.ProviderIds.FirstOrDefault(y => y.Key == "Tmdb").Value));
            episode.TVDB.Should().Be(baseItem.ProviderIds.FirstOrDefault(y => y.Key == "Tvdb").Value);
            episode.AudioStreams.Count.Should().Be(1);
            episode.SubtitleStreams.Count.Should().Be(1);
            episode.VideoStreams.Count.Should().Be(1);

            var audioStream = baseItem.MediaStreams.First(y => y.Type == MediaStreamType.Audio);
            episode.AudioStreams[0].BitRate.Should().Be(audioStream.BitRate);
            episode.AudioStreams[0].ChannelLayout.Should().Be(audioStream.ChannelLayout);
            episode.AudioStreams[0].Language.Should().Be(audioStream.Language);
            episode.AudioStreams[0].Channels.Should().Be(audioStream.Channels);
            episode.AudioStreams[0].Id.Should().NotBeNullOrWhiteSpace();
            episode.AudioStreams[0].Codec.Should().Be(audioStream.Codec);
            episode.AudioStreams[0].SampleRate.Should().Be(audioStream.SampleRate);

            var subtitleStream = baseItem.MediaStreams.First(y => y.Type == MediaStreamType.Subtitle);
            episode.SubtitleStreams[0].Language.Should().Be(subtitleStream.Language);
            episode.SubtitleStreams[0].Id.Should().NotBeNullOrWhiteSpace();
            episode.SubtitleStreams[0].Codec.Should().Be(subtitleStream.Codec);
            episode.SubtitleStreams[0].DisplayTitle.Should().Be(subtitleStream.DisplayTitle);
            episode.SubtitleStreams[0].IsDefault.Should().Be(subtitleStream.IsDefault);

            var videoStream = baseItem.MediaStreams.First(y => y.Type == MediaStreamType.Video);
            episode.VideoStreams[0].BitRate.Should().Be(videoStream.BitRate);
            episode.VideoStreams[0].Language.Should().Be(videoStream.Language);
            episode.VideoStreams[0].Channels.Should().Be(videoStream.Channels);
            episode.VideoStreams[0].Id.Should().NotBeNullOrWhiteSpace();
            episode.VideoStreams[0].AspectRatio.Should().Be(videoStream.AspectRatio);
            episode.VideoStreams[0].AverageFrameRate.Should().Be(videoStream.AverageFrameRate);
            episode.VideoStreams[0].Height.Should().Be(videoStream.Height);
            episode.VideoStreams[0].Width.Should().Be(videoStream.Width);
        }
    }
}
