﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EmbyStat.Common.Models;
using RestSharp;

namespace EmbyStat.Common.Extensions
{
    public static class RestRequestExtension
    {
        public static void AddItemQueryAsParameters(this IRestRequest request, ItemQuery query, string userId)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                request.AddQueryParameter("UserId", new Guid(userId).ToString());
            }

            if (!query.EnableTotalRecordCount)
            {
                request.AddQueryParameter("EnableTotalRecordCount", query.EnableTotalRecordCount.ToString());
            }

            if (query.SortOrder.HasValue)
            {
                request.AddQueryParameter("sortOrder", query.SortOrder?.ToString());
            }

            if (query.SeriesStatuses != null && query.SeriesStatuses.Any())
            {
                request.AddQueryParameter("SeriesStatuses", string.Join(',',query.SeriesStatuses));
            }

            if (query.Fields != null && query.Fields.Any())
            {
                request.AddQueryParameter("fields", string.Join(',', query.Fields));
            }

            if (query.Filters != null && query.Filters.Any())
            {
                request.AddQueryParameter("Filters", string.Join(',', query.Filters));
            }

            if (query.ImageTypes != null && query.ImageTypes.Any())
            {
                request.AddQueryParameter("ImageTypes", string.Join(',', query.ImageTypes));
            }

            if (query.AirDays != null && query.AirDays.Any())
            {
                request.AddQueryParameter("AirDays", string.Join(',', query.AirDays));
            }

            if (query.EnableImageTypes != null && query.EnableImageTypes.Any())
            {
                request.AddQueryParameter("EnableImageTypes", string.Join(',', query.EnableImageTypes));
            }

            if (query.LocationTypes != null && query.LocationTypes.Any())
            {
                request.AddQueryParameter("LocationTypes", string.Join(',', query.LocationTypes));
            }

            if (query.ExcludeLocationTypes != null && query.ExcludeLocationTypes.Any())
            {
                request.AddQueryParameter("ExcludeLocationTypes", string.Join(',', query.ExcludeLocationTypes));
            }

            request.AddIfNotNull("MinDateLastSaved", query.MinDateLastSaved);
            request.AddIfNotNull("MinDateLastSavedForUser", query.MinDateLastSaved);
            request.AddIfNotNullOrEmpty("ParentId", query.ParentId);
            request.AddIfNotNull("StartIndex", query.StartIndex);
            request.AddIfNotNull("Limit", query.Limit);
            request.AddIfNotNull("SortBy", query.SortBy);
            request.AddIfNotNull("Is3D", query.Is3D);
            request.AddIfNotNullOrEmpty("MinOfficialRating", query.MinOfficialRating);
            request.AddIfNotNullOrEmpty("MaxOfficialRating", query.MaxOfficialRating);
            request.AddIfNotNull("recursive", query.Recursive);
            request.AddIfNotNull("MinIndexNumber", query.MinIndexNumber);
            request.AddIfNotNull("EnableImages", query.EnableImages);
            request.AddIfNotNull("ImageTypeLimit", query.ImageTypeLimit);
            request.AddIfNotNull("CollapseBoxSetItems", query.CollapseBoxSetItems);
            request.AddIfNotNull("MediaTypes", query.MediaTypes);
            request.AddIfNotNull("Genres", query.Genres, '|');
            request.AddIfNotNull("Ids", query.Ids);
            request.AddIfNotNull("StudioIds", query.StudioIds, '|');
            request.AddIfNotNull("ExcludeItemTypes", query.ExcludeItemTypes);
            request.AddIfNotNull("IncludeItemTypes", query.IncludeItemTypes);
            request.AddIfNotNull("ArtistIds", query.ArtistIds);
            request.AddIfNotNull("IsPlayed", query.IsPlayed);
            request.AddIfNotNull("IsInBoxSet", query.IsInBoxSet);
            request.AddIfNotNull("PersonIds", query.PersonIds);
            request.AddIfNotNull("PersonTypes", query.PersonTypes);
            request.AddIfNotNull("Years", query.Years);
            request.AddIfNotNull("ParentIndexNumber", query.ParentIndexNumber);
            request.AddIfNotNull("HasParentalRating", query.HasParentalRating);
            request.AddIfNotNullOrEmpty("SearchTerm", query.SearchTerm);
            request.AddIfNotNull("MinCriticRating", query.MinCriticRating);
            request.AddIfNotNull("MinCommunityRating", query.MinCommunityRating);
            request.AddIfNotNull("MinPlayers", query.MinPlayers);
            request.AddIfNotNull("MaxPlayers", query.MaxPlayers);
            request.AddIfNotNullOrEmpty("NameStartsWithOrGreater", query.NameStartsWithOrGreater);
            request.AddIfNotNullOrEmpty("AlbumArtistStartsWithOrGreater", query.AlbumArtistStartsWithOrGreater);
            request.AddIfNotNull("IsMissing", query.IsMissing);
            request.AddIfNotNull("IsUnaired", query.IsUnaired);
            request.AddIfNotNull("IsVirtualUnaired", query.IsVirtualUnaired);
            request.AddIfNotNull("AiredDuringSeason", query.AiredDuringSeason);
        }

        private static void AddIfNotNullOrEmpty(this IRestRequest request, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                request.AddQueryParameter(key, value);
            }
        }

        private static void AddIfNotNull(this IRestRequest request, string key, bool? value)
        {
            if (value.HasValue)
            {
                request.AddQueryParameter(key, value.Value.ToString());
            }
        }

        private static void AddIfNotNull(this IRestRequest request, string key, int? value)
        {
            if (value.HasValue)
            {
                request.AddQueryParameter(key, value.Value.ToString());
            }
        }

        private static void AddIfNotNull(this IRestRequest request, string key, DateTime? value)
        {
            if (value.HasValue)
            {
                request.AddQueryParameter(key, value.Value.ToString("O"));
            }
        }

        private static void AddIfNotNull(this IRestRequest request, string key, double? value)
        {
            if (value.HasValue)
            {
                request.AddQueryParameter(key, value.Value.ToString(CultureInfo.InvariantCulture));
            }
        }

        private static void AddIfNotNull(this IRestRequest request, string key, IEnumerable<int> list)
        {
            request.AddIfNotNullOrEmpty(key, string.Join(',', list));
        }

        private static void AddIfNotNull(this IRestRequest request, string key, IEnumerable<string> list, char separator)
        {
            request.AddIfNotNullOrEmpty(key, string.Join(separator, list));
        }

        private static void AddIfNotNull(this IRestRequest request, string key, IEnumerable<string> list)
        {
            request.AddIfNotNull(key, list, ',');
        }
    }
}
