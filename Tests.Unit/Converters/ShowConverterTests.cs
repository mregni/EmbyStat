using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Show;
using FluentAssertions;
using Tests.Unit.Builders;
using Xunit;

namespace Tests.Unit.Converters
{

    public class ShowConverterTests
    {
        [Fact]
        public void ConvertToEpisode_Should_Convert_Virtual_Episode()
        {
            var show = new ShowBuilder("1", "1").Build();
            var showEpisode = show.Episodes.First();
            var showSeason = show.Seasons.First();

            var virtualEpisode = new VirtualEpisode(showEpisode, showSeason);
            var episode = virtualEpisode.ConvertToEpisode(show, showSeason);

            episode.Id.Should().Be(showEpisode.Id);
            episode.ShowId.Should().Be(show.Id);
            episode.ShowName.Should().Be(show.Name);
            episode.Name.Should().Be(showEpisode.Name);
            episode.LocationType.Should().Be(LocationType.Virtual);
            episode.IndexNumber.Should().Be(showEpisode.IndexNumber);
            episode.ParentId.Should().Be(showSeason.Id);
            // ReSharper disable once PossibleInvalidOperationException
            episode.PremiereDate.Should().Be(showEpisode.PremiereDate.Value.Date);
        }
    }
}
