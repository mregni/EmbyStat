using System;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Querying;

namespace EmbyStat.Common.Models;

public class ItemQuery
{
    public string UserId { get; set; }
    public string ParentId { get; set; }
    public int? StartIndex { get; set; }
    public int? Limit { get; set; }
    public ItemFields[] Fields { get; set; }
    public string[] MediaTypes { get; set; }
    public bool Recursive { get; set; }
    public string[] IncludeItemTypes { get; set; }
    public LocationType[] LocationTypes { get; set; }
    public LocationType[] ExcludeLocationTypes { get; set; }
    public bool? EnableImages { get; set; }
    public ImageType[] EnableImageTypes { get; set; }
    public bool EnableTotalRecordCount { get; set; }
    public DateTime? MinDateLastSaved { get; set; }
    public bool? IsPlayed { get; set; }
    public bool? EnableUserData { get; set; }
    public string[] Ids { get; set; }

    public ItemQuery()
    {
        LocationTypes = Array.Empty<LocationType>();
        ExcludeLocationTypes = Array.Empty<LocationType>();
        Fields = Array.Empty<ItemFields>();
        MediaTypes = Array.Empty<string>();
        EnableTotalRecordCount = true;
        IncludeItemTypes = Array.Empty<string>();
        EnableImageTypes = Array.Empty<ImageType>();
        Ids = Array.Empty<string>();
    }
}