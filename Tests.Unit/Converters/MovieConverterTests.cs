using System.Linq;
using EmbyStat.Clients.Base.Converters;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Helpers;
using FluentAssertions;
using Tests.Unit.Builders;
using Xunit;

namespace Tests.Unit.Converters
{
    public class MovieConverterTests
    {
        [Fact]
        public void ConvertToMovie_Should_Return_Movie()
        {
            var baseItem = new MovieBuilder("123")
                .AddAudioStream(new AudioStream
                {
                    BitRate = 1,
                    ChannelLayout = "test",
                    Channels = 12,
                    Codec = "codec",
                    Id = "123",
                    Language = "en",
                    SampleRate = 123
                })
                .AddSubtitleStream(new SubtitleStream
                {
                    Codec = "codec",
                    DisplayTitle = "title",
                    Id = "123",
                    Language = "en",
                    IsDefault = true
                })
                .AddVideoStream(new VideoStream
                {
                    Language = "en",
                    Channels = 12,
                    BitRate = 1,
                    Id = "123",
                    AspectRatio = "ratio",
                    AverageFrameRate = 1.2f,
                    Height = 123,
                    Width = 456
                })
                .BuildBaseItemDto();

            var movie = baseItem.ConvertToMovie("12");

            movie.Should().NotBeNull();
            movie.Id.Should().Be(baseItem.Id);
            movie.CollectionId.Should().Be("12");
            movie.Name.Should().Be(baseItem.Name);
            movie.ParentId.Should().Be(baseItem.ParentId);
            movie.OriginalTitle.Should().Be(baseItem.OriginalTitle);
            movie.DateCreated.Should().Be(baseItem.DateCreated);
            movie.Path.Should().Be(baseItem.Path);
            movie.SortName.Should().Be(baseItem.SortName);
            movie.RunTimeTicks.Should().Be(baseItem.RunTimeTicks);
            movie.Container.Should().Be(baseItem.Container);
            movie.CommunityRating.Should().Be(baseItem.CommunityRating);
            movie.MediaType.Should().Be(baseItem.MediaType);
            movie.OfficialRating.Should().Be(baseItem.OfficialRating);
            movie.PremiereDate.Should().Be(baseItem.PremiereDate);
            movie.ProductionYear.Should().Be(baseItem.ProductionYear);
            movie.Video3DFormat.Should().Be(baseItem.Video3DFormat);
            movie.Genres.Should().BeEquivalentTo(baseItem.Genres);
            movie.Primary.Should().Be(baseItem.ImageTags.FirstOrDefault(y => y.Key == ImageType.Primary).Value);
            movie.Thumb.Should().Be(baseItem.ImageTags.FirstOrDefault(y => y.Key == ImageType.Thumb).Value);
            movie.Logo.Should().Be(baseItem.ImageTags.FirstOrDefault(y => y.Key == ImageType.Logo).Value);
            movie.Banner.Should().Be(baseItem.ImageTags.FirstOrDefault(y => y.Key == ImageType.Banner).Value);
            movie.IMDB.Should().Be(baseItem.ProviderIds.FirstOrDefault(y => y.Key == "Imdb").Value);
            movie.TMDB.Should().Be(baseItem.ProviderIds.FirstOrDefault(y => y.Key == "Tmdb").Value);
            movie.TVDB.Should().Be(baseItem.ProviderIds.FirstOrDefault(y => y.Key == "Tvdb").Value);
            movie.AudioStreams.Count.Should().Be(1);
            movie.SubtitleStreams.Count.Should().Be(1);
            movie.VideoStreams.Count.Should().Be(1);

            var audioStream = baseItem.MediaStreams.First(y => y.Type == MediaStreamType.Audio);
            movie.AudioStreams[0].BitRate.Should().Be(audioStream.BitRate);
            movie.AudioStreams[0].ChannelLayout.Should().Be(audioStream.ChannelLayout);
            movie.AudioStreams[0].Language.Should().Be(audioStream.Language);
            movie.AudioStreams[0].Channels.Should().Be(audioStream.Channels);
            movie.AudioStreams[0].Id.Should().NotBeNullOrWhiteSpace();
            movie.AudioStreams[0].Codec.Should().Be(audioStream.Codec);
            movie.AudioStreams[0].SampleRate.Should().Be(audioStream.SampleRate);

            var subtitleStream = baseItem.MediaStreams.First(y => y.Type == MediaStreamType.Subtitle);
            movie.SubtitleStreams[0].Language.Should().Be(subtitleStream.Language);
            movie.SubtitleStreams[0].Id.Should().NotBeNullOrWhiteSpace();
            movie.SubtitleStreams[0].Codec.Should().Be(subtitleStream.Codec);
            movie.SubtitleStreams[0].DisplayTitle.Should().Be(subtitleStream.DisplayTitle);
            movie.SubtitleStreams[0].IsDefault.Should().Be(subtitleStream.IsDefault);

            var videoStream = baseItem.MediaStreams.First(y => y.Type == MediaStreamType.Video);
            movie.VideoStreams[0].BitRate.Should().Be(videoStream.BitRate);
            movie.VideoStreams[0].Language.Should().Be(videoStream.Language);
            movie.VideoStreams[0].Channels.Should().Be(videoStream.Channels);
            movie.VideoStreams[0].Id.Should().NotBeNullOrWhiteSpace();
            movie.VideoStreams[0].AspectRatio.Should().Be(videoStream.AspectRatio);
            movie.VideoStreams[0].AverageFrameRate.Should().Be(videoStream.AverageFrameRate);
            movie.VideoStreams[0].Height.Should().Be(videoStream.Height);
            movie.VideoStreams[0].Width.Should().Be(videoStream.Width);

            movie.People.Length.Should().Be(baseItem.People.Length);

        }
    }
}
