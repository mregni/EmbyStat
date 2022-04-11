using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models;

namespace EmbyStat.Common.Extensions;

public static class ItemQueryExtensions
{
    public static Dictionary<string, string> ConvertToStringDictionary(this ItemQuery query)
    {
        var paramList = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(query.UserId))
        {
            paramList.TryAdd("UserId", query.UserId);
        }

        if (query.EnableTotalRecordCount)
        {
            paramList.TryAdd("EnableTotalRecordCount", query.EnableTotalRecordCount.ToString());
        }

        if (query.Fields != null && query.Fields.Any())
        {
            paramList.TryAdd("Fields", string.Join(',', query.Fields));
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

        paramList.AddIfNotNull("MinDateLastSaved", query.MinDateLastSaved);
        paramList.AddIfNotNull("MinDateLastSavedForUser", query.MinDateLastSaved);
        paramList.AddIfNotNull("ParentId", query.ParentId);
        paramList.AddIfNotNull("StartIndex", query.StartIndex);
        paramList.AddIfNotNull("Limit", query.Limit);
        paramList.AddIfNotNull("Recursive", query.Recursive);
        paramList.AddIfNotNull("EnableImages", query.EnableImages);
        paramList.AddIfNotNull("MediaTypes", query.MediaTypes);
        paramList.AddIfNotNull("IncludeItemTypes", query.IncludeItemTypes);

        return paramList;
    }
}