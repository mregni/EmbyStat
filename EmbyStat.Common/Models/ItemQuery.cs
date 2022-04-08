using System;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Querying;

namespace EmbyStat.Common.Models
{
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

        public ItemQuery()
        {
            LocationTypes = new LocationType[] { };
            ExcludeLocationTypes = new LocationType[] { };
            Fields = new ItemFields[] { };
            MediaTypes = new string[] { };
            EnableTotalRecordCount = true;
            IncludeItemTypes = new string[] { };
            EnableImageTypes = new ImageType[] { };
        }
    }
}