using System;
using EmbyStat.Clients.Tmdb.Converter;
using FluentAssertions;
using TMDbLib.Objects.Search;
using Xunit;

namespace Tests.Unit.Converters
{
    public class EpisodeHelperTests
    {
        [Fact]
        public void ConvertToVirtualEpisode_Should_Return_Episode_With_Aired_Season()
        {
            var data = new TvSeasonEpisode
            {
                Id = 123,
                SeasonNumber = 1,
                EpisodeNumber = 11,
                AirDate = new DateTime(2000, 1, 1),
                Name = "ep name"
            };

            var virtualEpisode = data.ConvertToVirtualEpisode();

            virtualEpisode.Id.Should().Be(data.Id.ToString());
            virtualEpisode.EpisodeNumber.Should().Be(data.EpisodeNumber);
            virtualEpisode.SeasonNumber.Should().Be(data.SeasonNumber);
            virtualEpisode.FirstAired.Should().Be(data.AirDate);
            virtualEpisode.Name.Should().Be(data.Name);
        }
    }
}
