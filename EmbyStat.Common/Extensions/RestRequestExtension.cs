using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models;

namespace EmbyStat.Common.Extensions
{
    public static class RestRequestExtension
    {
        public static Dictionary<string, string> ConvertToStringDictionary(this ItemQuery query, string userId)
        {
            var paramList = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(userId))
            {
                paramList.TryAdd("UserId", new Guid(userId).ToString());
            }

            if (!query.EnableTotalRecordCount)
            {
                paramList.TryAdd("EnableTotalRecordCount", query.EnableTotalRecordCount.ToString());
            }

            if (query.SortOrder.HasValue)
            {
                paramList.TryAdd("sortOrder", query.SortOrder?.ToString());
            }

            if (query.SeriesStatuses != null && query.SeriesStatuses.Any())
            {
                paramList.TryAdd("SeriesStatuses", string.Join(',', query.SeriesStatuses));
            }

            if (query.Fields != null && query.Fields.Any())
            {
                paramList.TryAdd("fields", string.Join(',', query.Fields));
            }

            if (query.Filters != null && query.Filters.Any())
            {
                paramList.TryAdd("Filters", string.Join(',', query.Filters));
            }

            if (query.ImageTypes != null && query.ImageTypes.Any())
            {
                paramList.TryAdd("ImageTypes", string.Join(',', query.ImageTypes));
            }

            if (query.AirDays != null && query.AirDays.Any())
            {
                paramList.TryAdd("AirDays", string.Join(',', query.AirDays));
            }

            if (query.EnableImageTypes != null && query.EnableImageTypes.Any())
            {
                paramList.TryAdd("EnableImageTypes", string.Join(',', query.EnableImageTypes));
            }

            if (query.LocationTypes != null && query.LocationTypes.Any())
            {
                paramList.TryAdd("LocationTypes", string.Join(',', query.LocationTypes));
            }

            if (query.ExcludeLocationTypes != null && query.ExcludeLocationTypes.Any())
            {
                paramList.TryAdd("ExcludeLocationTypes", string.Join(',', query.ExcludeLocationTypes));
            }

            paramList.TryAdd("ExcludeLocationTypes", string.Join(',', query.ExcludeLocationTypes));

            paramList.AddIfNotNull("MinDateLastSaved", query.MinDateLastSaved);
            paramList.AddIfNotNull("MinDateLastSavedForUser", query.MinDateLastSaved);
            paramList.AddIfNotNull("ParentId", query.ParentId);
            paramList.AddIfNotNull("StartIndex", query.StartIndex);
            paramList.AddIfNotNull("Limit", query.Limit);
            paramList.AddIfNotNull("SortBy", query.SortBy);
            paramList.AddIfNotNull("Is3D", query.Is3D);
            paramList.AddIfNotNull("MinOfficialRating", query.MinOfficialRating);
            paramList.AddIfNotNull("MaxOfficialRating", query.MaxOfficialRating);
            paramList.AddIfNotNull("recursive", query.Recursive);
            paramList.AddIfNotNull("MinIndexNumber", query.MinIndexNumber);
            paramList.AddIfNotNull("EnableImages", query.EnableImages);
            paramList.AddIfNotNull("ImageTypeLimit", query.ImageTypeLimit);
            paramList.AddIfNotNull("CollapseBoxSetItems", query.CollapseBoxSetItems);
            paramList.AddIfNotNull("MediaTypes", query.MediaTypes);
            paramList.AddIfNotNull("Genres", query.Genres, '|');
            paramList.AddIfNotNull("Ids", query.Ids);
            paramList.AddIfNotNull("StudioIds", query.StudioIds, '|');
            paramList.AddIfNotNull("ExcludeItemTypes", query.ExcludeItemTypes);
            paramList.AddIfNotNull("IncludeItemTypes", query.IncludeItemTypes);
            paramList.AddIfNotNull("ArtistIds", query.ArtistIds);
            paramList.AddIfNotNull("IsPlayed", query.IsPlayed);
            paramList.AddIfNotNull("IsInBoxSet", query.IsInBoxSet);
            paramList.AddIfNotNull("PersonIds", query.PersonIds);
            paramList.AddIfNotNull("PersonTypes", query.PersonTypes);
            paramList.AddIfNotNull("Years", query.Years);
            paramList.AddIfNotNull("ParentIndexNumber", query.ParentIndexNumber);
            paramList.AddIfNotNull("HasParentalRating", query.HasParentalRating);
            paramList.AddIfNotNull("SearchTerm", query.SearchTerm);
            paramList.AddIfNotNull("MinCriticRating", query.MinCriticRating);
            paramList.AddIfNotNull("MinCommunityRating", query.MinCommunityRating);
            paramList.AddIfNotNull("MinPlayers", query.MinPlayers);
            paramList.AddIfNotNull("MaxPlayers", query.MaxPlayers);
            paramList.AddIfNotNull("NameStartsWithOrGreater", query.NameStartsWithOrGreater);
            paramList.AddIfNotNull("AlbumArtistStartsWithOrGreater", query.AlbumArtistStartsWithOrGreater);
            paramList.AddIfNotNull("IsMissing", query.IsMissing);
            paramList.AddIfNotNull("IsUnaired", query.IsUnaired);
            paramList.AddIfNotNull("IsVirtualUnaired", query.IsVirtualUnaired);
            paramList.AddIfNotNull("AiredDuringSeason", query.AiredDuringSeason);

            return paramList;
        }
    }
}
