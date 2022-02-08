using System;
using System.Globalization;
using System.Linq;
using EmbyStat.Common.Models;
using FluentAssertions;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Querying;
using RestSharp;
using Xunit;

namespace Tests.Unit.Extensions
{
    public class RestRequestExtensionTests
    {
        [Fact]
        public void AddItemQueryAsParameters_Should_Add_All_Parameters_To_Request()
        {
            var request = new RestRequest();

            var query = new ItemQuery
            {
                ParentId = "01",
                AirDays = new[] { DayOfWeek.Friday, DayOfWeek.Monday },
                AiredDuringSeason = 1,
                AlbumArtistStartsWithOrGreater = "test",
                ArtistIds = new[] { "1" },
                CollapseBoxSetItems = true,
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Box },
                EnableImages = true,
                EnableTotalRecordCount = false,
                ExcludeItemTypes = new[] { "Movies" },
                ExcludeLocationTypes = new[] { LocationType.FileSystem, LocationType.Virtual },
                Fields = new[] { ItemFields.Bitrate, ItemFields.AirTime },
                Filters = new[] { ItemFilter.IsFavorite },
                Genres = new[] { "action" },
                HasParentalRating = true,
                Ids = new[] { "1" },
                ImageTypeLimit = 10,
                ImageTypes = new[] { ImageType.Banner, ImageType.Art },
                IncludeIndexContainers = true,
                IncludeItemTypes = new[] { "1" },
                Is3D = true,
                IsInBoxSet = false,
                IsMissing = false,
                IsPlayed = true,
                IsUnaired = true,
                IsVirtualUnaired = true,
                Limit = 10,
                LocationTypes = new[] { LocationType.Virtual, LocationType.FileSystem },
                MaxOfficialRating = "MP-12",
                MaxPlayers = 10,
                MaxPremiereDate = new DateTime(2001, 1, 1),
                MediaTypes = new[] { "movies" },
                MinCommunityRating = 0.2,
                MinCriticRating = 1.2,
                MinIndexNumber = 0,
                MinOfficialRating = "MP-1",
                MinPlayers = 1,
                MinPremiereDate = new DateTime(1990, 1, 1),
                NameLessThan = "zz",
                NameStartsWith = "a",
                NameStartsWithOrGreater = "aa",
                ParentIndexNumber = 1,
                PersonIds = new[] { "1" },
                PersonTypes = new[] { "actor" },
                Recursive = false,
                SearchTerm = "test",
                SeriesStatuses = new[] { SeriesStatus.Continuing, SeriesStatus.Ended },
                SortBy = new[] { "Name", "Id" },
                SortOrder = SortOrder.Ascending,
                StartIndex = 1,
                StudioIds = new[] { "1" },
                UserId = "10",
                Years = new[] { 2000 }
            };

            request.AddItemQueryAsParameters(query, "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246");
            request.Parameters.Count.Should().Be(48);

            var parameters = request.Parameters;
            parameters.Single(x => x.Name == "UserId").Value.As<string>().Should().Be("fa89fb6c-f3b7-4cc5-bc17-9522e3b94246");
            parameters.Single(x => x.Name == "EnableTotalRecordCount").Value.As<string>().Should().Be("False");
            parameters.Single(x => x.Name == "sortOrder").Value.As<string>().Should().Be("Ascending");
            parameters.Single(x => x.Name == "SeriesStatuses").Value.As<string>().Should().Be("Continuing,Ended");
            parameters.Single(x => x.Name == "fields").Value.As<string>().Should().Be("Bitrate,AirTime");
            parameters.Single(x => x.Name == "Filters").Value.As<string>().Should().Be("IsFavorite");
            parameters.Single(x => x.Name == "ImageTypes").Value.As<string>().Should().Be("Banner,Art");
            parameters.Single(x => x.Name == "AirDays").Value.As<string>().Should().Be("Friday,Monday");
            parameters.Single(x => x.Name == "EnableImageTypes").Value.As<string>().Should().Be("Banner,Box");
            parameters.Single(x => x.Name == "LocationTypes").Value.As<string>().Should().Be("Virtual,FileSystem");
            parameters.Single(x => x.Name == "ExcludeLocationTypes").Value.As<string>().Should().Be("FileSystem,Virtual");
            parameters.Single(x => x.Name == "ParentId").Value.As<string>().Should().Be(query.ParentId);
            parameters.Single(x => x.Name == "StartIndex").Value.As<string>().Should().Be(query.StartIndex.ToString());
            parameters.Single(x => x.Name == "Limit").Value.As<string>().Should().Be(query.Limit.ToString());
            parameters.Single(x => x.Name == "SortBy").Value.As<string>().Should().Be(string.Join(',', query.SortBy));
            parameters.Single(x => x.Name == "Is3D").Value.As<string>().Should().Be(query.Is3D.ToString());
            parameters.Single(x => x.Name == "MinOfficialRating").Value.As<string>().Should().Be(query.MinOfficialRating);
            parameters.Single(x => x.Name == "MaxOfficialRating").Value.As<string>().Should().Be(query.MaxOfficialRating);
            parameters.Single(x => x.Name == "recursive").Value.As<string>().Should().Be(query.Recursive.ToString());
            parameters.Single(x => x.Name == "MinIndexNumber").Value.As<string>().Should().Be(query.MinIndexNumber.ToString());
            parameters.Single(x => x.Name == "EnableImages").Value.As<string>().Should().Be(query.EnableImages.ToString());
            parameters.Single(x => x.Name == "ImageTypeLimit").Value.As<string>().Should().Be(query.ImageTypeLimit.ToString());
            parameters.Single(x => x.Name == "CollapseBoxSetItems").Value.As<string>().Should().Be(query.CollapseBoxSetItems.ToString());
            parameters.Single(x => x.Name == "MediaTypes").Value.As<string>().Should().Be(string.Join(',', query.MediaTypes));
            parameters.Single(x => x.Name == "Genres").Value.As<string>().Should().Be(string.Join('|', query.Genres));
            parameters.Single(x => x.Name == "Ids").Value.As<string>().Should().Be(string.Join(',', query.Ids));
            parameters.Single(x => x.Name == "StudioIds").Value.As<string>().Should().Be(string.Join('|', query.StudioIds));
            parameters.Single(x => x.Name == "ExcludeItemTypes").Value.As<string>().Should().Be(string.Join(',', query.ExcludeItemTypes));
            parameters.Single(x => x.Name == "IncludeItemTypes").Value.As<string>().Should().Be(string.Join(',', query.IncludeItemTypes));
            parameters.Single(x => x.Name == "ArtistIds").Value.As<string>().Should().Be(string.Join(',', query.ArtistIds));
            parameters.Single(x => x.Name == "IsPlayed").Value.As<string>().Should().Be(query.IsPlayed.ToString());
            parameters.Single(x => x.Name == "IsInBoxSet").Value.As<string>().Should().Be(query.IsInBoxSet.ToString());
            parameters.Single(x => x.Name == "PersonIds").Value.As<string>().Should().Be(string.Join(',', query.PersonIds));
            parameters.Single(x => x.Name == "PersonTypes").Value.As<string>().Should().Be(string.Join(',', query.PersonTypes));
            parameters.Single(x => x.Name == "Years").Value.As<string>().Should().Be(string.Join(',', query.Years));
            parameters.Single(x => x.Name == "ParentIndexNumber").Value.As<string>().Should().Be(query.ParentIndexNumber.ToString());
            parameters.Single(x => x.Name == "HasParentalRating").Value.As<string>().Should().Be(query.HasParentalRating.ToString());
            parameters.Single(x => x.Name == "SearchTerm").Value.As<string>().Should().Be(query.SearchTerm);
            parameters.Single(x => x.Name == "MinCriticRating").Value.As<string>().Should().Be(query.MinCriticRating.Value.ToString(CultureInfo.InvariantCulture));
            parameters.Single(x => x.Name == "MinCommunityRating").Value.As<string>().Should().Be(query.MinCommunityRating.Value.ToString(CultureInfo.InvariantCulture));
            parameters.Single(x => x.Name == "MinPlayers").Value.As<string>().Should().Be(query.MinPlayers.ToString());
            parameters.Single(x => x.Name == "MaxPlayers").Value.As<string>().Should().Be(query.MaxPlayers.ToString());
            parameters.Single(x => x.Name == "NameStartsWithOrGreater").Value.As<string>().Should().Be(query.NameStartsWithOrGreater);
            parameters.Single(x => x.Name == "AlbumArtistStartsWithOrGreater").Value.As<string>().Should().Be(query.AlbumArtistStartsWithOrGreater);
            parameters.Single(x => x.Name == "IsMissing").Value.As<string>().Should().Be(query.IsMissing.ToString());
            parameters.Single(x => x.Name == "IsUnaired").Value.As<string>().Should().Be(query.IsUnaired.ToString());
            parameters.Single(x => x.Name == "IsVirtualUnaired").Value.As<string>().Should().Be(query.IsVirtualUnaired.ToString());
            parameters.Single(x => x.Name == "AiredDuringSeason").Value.As<string>().Should().Be(query.AiredDuringSeason.Value.ToString());
        }

        [Fact]
        public void AddItemQueryAsParameters_Should_Have_Default_Values()
        {
            var request = new RestRequest();

            var query = new ItemQuery();
            request.AddItemQueryAsParameters(query, "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246");
            request.Parameters.Count.Should().Be(2);

            var parameters = request.Parameters;
            parameters.Single(x => x.Name == "UserId").Value.As<string>().Should().Be("fa89fb6c-f3b7-4cc5-bc17-9522e3b94246");
            parameters.Single(x => x.Name == "recursive").Value.As<string>().Should().Be("False");

        }
    }
}
