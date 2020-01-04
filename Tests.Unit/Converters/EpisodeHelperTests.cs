using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Clients.Tvdb.Converter;
using EmbyStat.Clients.Tvdb.Models;
using FluentAssertions;
using Xunit;

namespace Tests.Unit.Converters
{
    public class EpisodeHelperTests
    {
        [Fact]
        public void ConvertToVirtualEpisode_Should_Return_Episode_With_Dvd_Info()
        {
            var data = new Data
            {
                Id = 123,
                DvdSeason = 2,
                DvdEpisodeNumber = 10,
                FirstAired = new DateTime(2000, 1, 1).ToLongDateString(),
                EpisodeName = "ep name"
            };

            var virtualEpisode = data.ConvertToVirtualEpisode();

            virtualEpisode.Id.Should().Be(data.Id.ToString());
            // ReSharper disable once PossibleInvalidOperationException
            virtualEpisode.EpisodeNumber.Should().Be(Convert.ToInt32(Math.Floor(data.DvdEpisodeNumber.Value)));
            // ReSharper disable once PossibleInvalidOperationException
            virtualEpisode.SeasonNumber.Should().Be(Convert.ToInt32(Math.Floor(data.DvdSeason.Value)));
            virtualEpisode.FirstAired.Should().Be(DateTime.Parse(data.FirstAired));
            virtualEpisode.Name.Should().Be(data.EpisodeName);
        }

        [Fact]
        public void ConvertToVirtualEpisode_Should_Return_Episode_With_Aired_Info()
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
    }
}
