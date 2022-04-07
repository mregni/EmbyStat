using System;
using System.Collections.Generic;
using System.Globalization;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models;
using FluentAssertions;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Querying;
using Xunit;

namespace Tests.Unit.Extensions
{
    public class RestRequestExtensionTests
    {
        [Fact]
        public void AddItemQueryAsParameters_Should_Add_All_Parameters_To_Request()
        {
            var query = new ItemQuery
            {
                ParentId = "01",
                AirDays = new[] {DayOfWeek.Friday, DayOfWeek.Monday},
                AiredDuringSeason = 1,
                AlbumArtistStartsWithOrGreater = "test",
                ArtistIds = new[] {"1"},
                CollapseBoxSetItems = true,
                EnableImageTypes = new[] {ImageType.Banner, ImageType.Box},
                EnableImages = true,
                EnableTotalRecordCount = false,
                ExcludeItemTypes = new[] {"Movies"},
                ExcludeLocationTypes = new[] {LocationType.Virtual},
                Fields = new[] {ItemFields.Bitrate, ItemFields.AirTime},
                Filters = new[] {ItemFilter.IsFavorite},
                Genres = new[] {"action"},
                HasParentalRating = true,
                Ids = new[] {"1"},
                ImageTypeLimit = 10,
                ImageTypes = new[] {ImageType.Banner, ImageType.Art},
                IncludeIndexContainers = true,
                IncludeItemTypes = new[] {"1"},
                Is3D = true,
                IsInBoxSet = false,
                IsMissing = false,
                IsPlayed = true,
                IsUnaired = true,
                IsVirtualUnaired = true,
                Limit = 10,
                LocationTypes = new[] {LocationType.FileSystem},
                MaxOfficialRating = "MP-12",
                MaxPlayers = 10,
                MaxPremiereDate = new DateTime(2001, 1, 1),
                MediaTypes = new[] {"movies"},
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
                PersonIds = new[] {"1"},
                PersonTypes = new[] {"actor"},
                Recursive = false,
                SearchTerm = "test",
                SeriesStatuses = new[] {SeriesStatus.Continuing, SeriesStatus.Ended},
                SortBy = new[] {"Name", "Id"},
                SortOrder = SortOrder.Ascending,
                StartIndex = 1,
                StudioIds = new[] {"1"},
                UserId = "fa89fb6c-f3b7-4cc5-bc17-9522e3b94246",
                Years = new[] {2000}
            };

            var dict = query.ConvertToStringDictionary();

            dict.Count.Should().Be(48);
            DictionaryChecker(dict, "UserId", query.UserId);
            DictionaryChecker(dict, "EnableTotalRecordCount", query.EnableTotalRecordCount.ToString());
            DictionaryChecker(dict, "sortOrder", query.SortOrder.ToString());
            DictionaryChecker(dict, "SeriesStatuses", string.Join(',', query.SeriesStatuses));
            DictionaryChecker(dict, "fields", string.Join(',', query.Fields));
            DictionaryChecker(dict, "Filters", string.Join(',', query.Filters));
            DictionaryChecker(dict, "ImageTypes", string.Join(',', query.ImageTypes));
            DictionaryChecker(dict, "AirDays", string.Join(',', query.AirDays));
            DictionaryChecker(dict, "EnableImageTypes", string.Join(',', query.EnableImageTypes));
            DictionaryChecker(dict, "LocationTypes",string.Join(',', query.LocationTypes));
            DictionaryChecker(dict, "ExcludeLocationTypes", string.Join(',', query.ExcludeLocationTypes));
            DictionaryChecker(dict, "ParentId", query.ParentId);
            DictionaryChecker(dict, "StartIndex", query.StartIndex.ToString());
            DictionaryChecker(dict, "Limit", query.Limit.ToString());
            DictionaryChecker(dict, "SortBy", string.Join('|', query.SortBy));
            DictionaryChecker(dict, "Is3D", query.Is3D.ToString());
            DictionaryChecker(dict, "MinOfficialRating", query.MinOfficialRating);
            DictionaryChecker(dict, "MaxOfficialRating", query.MaxOfficialRating);
            DictionaryChecker(dict, "recursive", query.Recursive.ToString());
            DictionaryChecker(dict, "MinIndexNumber", query.MinIndexNumber.ToString());
            DictionaryChecker(dict, "EnableImages", query.EnableImages.ToString());
            DictionaryChecker(dict, "ImageTypeLimit", query.ImageTypeLimit.ToString());
            DictionaryChecker(dict, "CollapseBoxSetItems", query.CollapseBoxSetItems.ToString());
            DictionaryChecker(dict, "MediaTypes", string.Join(',', query.MediaTypes));
            DictionaryChecker(dict, "Genres", string.Join('|', query.Genres));
            DictionaryChecker(dict, "Ids", string.Join(',', query.Ids));
            DictionaryChecker(dict, "StudioIds", string.Join('|', query.StudioIds));
            DictionaryChecker(dict, "ExcludeItemTypes", string.Join(',', query.ExcludeItemTypes));
            DictionaryChecker(dict, "IncludeItemTypes", string.Join(',', query.IncludeItemTypes));
            DictionaryChecker(dict, "ArtistIds", string.Join(',', query.ArtistIds));
            DictionaryChecker(dict, "IsPlayed", query.IsPlayed.ToString());
            DictionaryChecker(dict, "IsInBoxSet", query.IsInBoxSet.ToString());
            DictionaryChecker(dict, "PersonIds", string.Join(',', query.PersonIds));
            DictionaryChecker(dict, "PersonTypes", string.Join(',', query.PersonTypes));
            DictionaryChecker(dict, "Years", string.Join(',', query.Years));
            DictionaryChecker(dict, "ParentIndexNumber", query.ParentIndexNumber.ToString());
            DictionaryChecker(dict, "HasParentalRating", query.HasParentalRating.ToString());
            DictionaryChecker(dict, "SearchTerm", query.SearchTerm);
            DictionaryChecker(dict, "MinCriticRating", query.MinCriticRating.Value.ToString(CultureInfo.InvariantCulture));
            DictionaryChecker(dict, "MinPlayers", query.MinPlayers.ToString());
            DictionaryChecker(dict, "MaxPlayers", query.MaxPlayers.ToString());
            DictionaryChecker(dict, "NameStartsWithOrGreater", query.NameStartsWithOrGreater);
            DictionaryChecker(dict, "AlbumArtistStartsWithOrGreater", query.AlbumArtistStartsWithOrGreater);
            DictionaryChecker(dict, "IsMissing", query.IsMissing.ToString());
            DictionaryChecker(dict, "IsUnaired", query.IsUnaired.ToString());
            DictionaryChecker(dict, "IsVirtualUnaired", query.IsVirtualUnaired.ToString());
            DictionaryChecker(dict, "AiredDuringSeason", query.AiredDuringSeason.Value.ToString());
        }

        [Fact]
        public void AddItemQueryAsParameters_Should_Have_Default_Values()
        {
            var query = new ItemQuery();
            var dict = query.ConvertToStringDictionary();
            dict.Count.Should().Be(1);

            DictionaryChecker(dict, "recursive", query.Recursive.ToString());
        }
        
        private void DictionaryChecker(IReadOnlyDictionary<string, string> dict, string index, string value)
        {
            dict.ContainsKey(index).Should().BeTrue();
            dict[index].Should().Be(value);  
        }
    }
}