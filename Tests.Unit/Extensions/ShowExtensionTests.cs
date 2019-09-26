using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using FluentAssertions;
using Tests.Unit.Builders;
using Xunit;

namespace Tests.Unit.Extensions
{
    public class ShowExtensionTests
    {
        private readonly Show _show;
        public ShowExtensionTests()
        {
            _show = new ShowBuilder(0, "123")
                .AddMissingEpisodes(2, 2)
                .AddSeason(0, 2)
                .AddSeason(0, 3)
                .AddSeason(2, 3)
                .AddSeason(3, 2)
                .Build();
        }


        [Fact]
        public void CountShouldOnlyIncludeNormalEpisodesAndNeverSpecials()
        {
            var count = _show.GetNonSpecialEpisodeCount(false);
            count.Should().Be(7);
        }

        [Fact]
        public void CountShouldIncludeMissingEpisodesAndNeverSpecials()
        {
            var count = _show.GetNonSpecialEpisodeCount(true);
            count.Should().Be(9);
        }

        [Fact]
        public void CountShouldOnlyIncludeNormalSeasons()
        {
            var count = _show.GetNonSpecialSeasonCount();
            count.Should().Be(3);
        }

        [Fact]
        public void CountShouldOnlyIncludeVirtualEpisodes()
        {
            var count = _show.GetMissingEpisodeCount();
            count.Should().Be(2);
        }

        [Fact]
        public void ShouldOnlyReturnVirtualEpisodes()
        {
            var missingEpisodes = _show.GetMissingEpisodes().ToList();
            missingEpisodes.Should().NotBeNull();
            missingEpisodes.Should().NotContainNulls();
            missingEpisodes.Count.Should().Be(2);

            missingEpisodes[0].Id.Should().Be(0);
            missingEpisodes[0].SeasonNumber.Should().Be(2);
            missingEpisodes[0].EpisodeNumber.Should().Be(0);

            missingEpisodes[1].Id.Should().Be(1);
            missingEpisodes[1].SeasonNumber.Should().Be(2);
            missingEpisodes[1].EpisodeNumber.Should().Be(1);
        }

        [Fact]
        public void ShouldReturnTotalShowSizeInMb()
        {
            var totalSize = _show.GetShowSize();
            totalSize.Should().Be(1212);
        }
    }
}
