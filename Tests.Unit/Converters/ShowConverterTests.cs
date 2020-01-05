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
                CommunityRating = 1.2f,
                DateCreated = new DateTimeOffset(2000, 1, 1, 1, 1, 1, TimeSpan.Zero),
                ProviderIds = new Dictionary<string, string>
                {
                    { "Imdb", "1234" },
                    { "Tmdb", "1234" },
                    { "Tvdb", "1234" }
                },
                OfficialRating = "16TV",
                PremiereDate = new DateTimeOffset(2000, 1, 1, 1, 1, 1, TimeSpan.Zero),
                ProductionYear = 2001,
                RunTimeTicks = 1000,
                SortName = "2 broke girls",
                Status = "Ended",
                Genres = new []{ "Action" },
                People = new []
                {
                    new BaseItemPerson
                    {
                        Id = "123",
                        Name = "Gimli",
                        Type = PersonType.Actor
                    } 
                }
            };

            var show = data.ConvertToShow("123");
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
            show.TMDB.Should().Be(data.ProviderIds.FirstOrDefault(y => y.Key == "Tmdb").Value);
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
        public void ConvertToEpisode_Should_Convert_Virtual_Episode()
        {
            var show = new ShowBuilder("1", "1").Build();
            var showEpisode = show.Episodes.First();
            var showSeason = show.Seasons.First();

            var virtualEpisode = new VirtualEpisode(showEpisode, showSeason);
            var episode = virtualEpisode.ConvertToEpisode(show, showSeason);

            episode.ShowId.Should().Be(show.Id);
            episode.ShowName.Should().Be(show.Name);
            episode.Name.Should().Be(showEpisode.Name);
            episode.LocationType.Should().Be(LocationType.Virtual);
            episode.IndexNumber.Should().Be(showEpisode.IndexNumber);
            episode.ParentId.Should().Be(showSeason.Id);
            // ReSharper disable once PossibleInvalidOperationException
            episode.PremiereDate.Should().Be(showEpisode.PremiereDate);
        }
    }
}
