using System;
using System.Linq;
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
            _show = new ShowBuilder(Guid.NewGuid().ToString(), "123")
                .AddSeason(0, 2)
                .AddSeason(2, 3)
                .AddSeason(3, 2)
                .AddMissingEpisodes(2, 2)
                .Build();
        }


        [Fact]
        public void GetNonSpecialEpisodeCount_Should_Only_Include_Normal_Episodes_And_Never_Specials()
        {
            var count = _show.GetNonSpecialEpisodeCount(false);
            count.Should().Be(7);
        }

        [Fact]
        public void GetNonSpecialEpisodeCount_Should_Include_Missing_Episodes_And_Never_Specials()
        {
            var count = _show.GetNonSpecialEpisodeCount(true);
            count.Should().Be(9);
        }

        [Fact]
        public void GetNonSpecialSeasonCount_Should_Only_Include_Normal_Seasons()
        {
            var count = _show.GetNonSpecialSeasonCount();
            count.Should().Be(3);
        }

        [Fact]
        public void GetMissingEpisodeCount_Should_Only_Include_Virtual_Episodes()
        {
            var count = _show.GetMissingEpisodeCount();
            count.Should().Be(2);
        }

        [Fact]
        public void GetMissingEpisodes_Should_Only_Return_Virtual_Episodes()
        {
            var missingEpisodes = _show.GetMissingEpisodes().ToList();
            missingEpisodes.Should().NotBeNull();
            missingEpisodes.Should().NotContainNulls();
            missingEpisodes.Count.Should().Be(2);

            missingEpisodes[0].SeasonNumber.Should().Be(2);
            missingEpisodes[0].EpisodeNumber.Should().Be(0);

            missingEpisodes[1].SeasonNumber.Should().Be(2);
            missingEpisodes[1].EpisodeNumber.Should().Be(1);
        }

        [Fact]
        public void GetShowSize_Should_Return_Total_Show_Size_In_Mb()
        {
            var totalSize = _show.GetShowSize();
            totalSize.Should().Be(909);
        }

        [Fact]
        public void NeedsShowSync_Should_Return_True_If_Show_Needs_Syncing()
        {
            var show = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
            var needsUpdate = show.NeedsShowSync();

            needsUpdate.Should().BeTrue();
        }

        [Fact]
        public void NeedsShowSync_Should_Return_True_If_Show_Failed_Sync()
        {
            var show = new ShowBuilder(Guid.NewGuid().ToString(), "1")
                .AddTvdbSynced(true)
                .AddFailedSync(true).Build();
            var needsUpdate = show.NeedsShowSync();

            needsUpdate.Should().BeTrue();
        }

        [Fact]
        public void NeedsShowSync_Should_Return_False_If_Show_Already_Synced()
        {
            var show = new ShowBuilder(Guid.NewGuid().ToString(), "1")
                .AddTvdbSynced(true).Build();
            var needsUpdate = show.NeedsShowSync();

            needsUpdate.Should().BeFalse();
        }

        [Fact]
        public void HasShowChangedEpisode_Should_Return_True_If_Show_Has_Changed_Episodes()
        {
            var oldShow = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
            var newShow = new ShowBuilder(Guid.NewGuid().ToString(), oldShow.Id)
                .AddEpisode(new EpisodeBuilder(Guid.NewGuid().ToString(), oldShow.Id, "1").Build())
                .Build();

            var changed = newShow.HasShowChangedEpisodes(oldShow);
            changed.Should().BeTrue();
        }

        [Fact]
        public void HasShowChangedEpisode_Should_Return_True_If_Show_Is_New()
        {
            var newShow = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
            var changed = newShow.HasShowChangedEpisodes(null);
            changed.Should().BeTrue();
        }

        [Fact]
        public void HasShowChangedEpisode_Should_Return_False_If_Shows_Are_Equal()
        {
            var oldShow = new ShowBuilder(Guid.NewGuid().ToString(), "1").Build();
            var changed = oldShow.HasShowChangedEpisodes(oldShow);
            changed.Should().BeFalse();
        }
    }
}
