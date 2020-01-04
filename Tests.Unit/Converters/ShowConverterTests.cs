using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Models.Show;
using FluentAssertions;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using Tests.Unit.Builders;
using Xunit;
using LocationType = EmbyStat.Common.Enums.LocationType;

namespace Tests.Unit.Converters
{

    public class ShowConverterTests
    {
        [Fact]
        public void ConvertToShow_Should_Return_A_Show()
        {
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
                CommunityRating = 1.2f
            };
            var show = data.ConvertToShow("123");
        }

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
