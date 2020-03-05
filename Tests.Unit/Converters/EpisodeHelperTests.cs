using System;
using EmbyStat.Clients.Tvdb.Converter;
using EmbyStat.Clients.Tvdb.Models;
using FluentAssertions;
using Xunit;

namespace Tests.Unit.Converters
{
    public class EpisodeHelperTests
    {
        [Fact]
        public void ConvertToVirtualEpisode_Should_Return_Episode_With_Aired_Season()
        {
            var data = new Data
            {
                Id = 123,
                DvdSeason = null,
                DvdEpisodeNumber = null,
                AiredSeason = 1,
                AiredEpisodeNumber = 11,
                FirstAired = new DateTime(2000, 1, 1).ToLongDateString(),
                EpisodeName = "ep name"
            };

            var virtualEpisode = data.ConvertToVirtualEpisode();

            virtualEpisode.Id.Should().Be(data.Id.ToString());
            // ReSharper disable once PossibleInvalidOperationException
            virtualEpisode.EpisodeNumber.Should().Be(data.AiredEpisodeNumber);
            // ReSharper disable once PossibleInvalidOperationException
            virtualEpisode.SeasonNumber.Should().Be(data.AiredSeason);
            virtualEpisode.FirstAired.Should().Be(DateTime.Parse(data.FirstAired));
            virtualEpisode.Name.Should().Be(data.EpisodeName);
        }

        [Fact]
        public void ConvertToVirtualEpisode_Should_Return_Episode_With_Dvd_Info_Because_Aired_Is_To_Big()
        {
            var data = new Data
            {
                Id = 123,
                DvdSeason = 2,
                DvdEpisodeNumber = 10,
                AiredSeason = 1950,
                AiredEpisodeNumber = 11,
                FirstAired = new DateTime(2000, 1, 1).ToLongDateString(),
                EpisodeName = "ep name"
            };

            var virtualEpisode = data.ConvertToVirtualEpisode();

            virtualEpisode.Id.Should().Be(data.Id.ToString());
            // ReSharper disable once PossibleInvalidOperationException
            virtualEpisode.EpisodeNumber.Should().Be(data.AiredEpisodeNumber);
            // ReSharper disable once PossibleInvalidOperationException
            virtualEpisode.SeasonNumber.Should().Be(Convert.ToInt32(Math.Floor(data.DvdSeason ?? 0)));
            virtualEpisode.FirstAired.Should().Be(DateTime.Parse(data.FirstAired));
            virtualEpisode.Name.Should().Be(data.EpisodeName);
        }

        [Fact]
        public void ConvertToVirtualEpisode_Should_Return_Episode_With_Aired_Season_Fallback()
        {
            var data = new Data
            {
                Id = 123,
                DvdSeason = null,
                DvdEpisodeNumber = 10,
                AiredSeason = 1000,
                AiredEpisodeNumber = 11,
                FirstAired = new DateTime(2000, 1, 1).ToLongDateString(),
                EpisodeName = "ep name"
            };

            var virtualEpisode = data.ConvertToVirtualEpisode();

            virtualEpisode.Id.Should().Be(data.Id.ToString());
            // ReSharper disable once PossibleInvalidOperationException
            virtualEpisode.EpisodeNumber.Should().Be(data.AiredEpisodeNumber);
            // ReSharper disable once PossibleInvalidOperationException
            virtualEpisode.SeasonNumber.Should().Be(data.AiredSeason);
            virtualEpisode.FirstAired.Should().Be(DateTime.Parse(data.FirstAired));
            virtualEpisode.Name.Should().Be(data.EpisodeName);
        }
    }
}
