using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Querying;

namespace EmbyStat.Clients.EmbyClient.Model
{
    public class ItemsByNameQuery
    {
        public bool? EnableImages { get; set; }
        public bool? IsPlayed { get; set; }
        public string NameLessThan { get; set; }
        public string NameStartsWith { get; set; }
        public string NameStartsWithOrGreater { get; set; }
        public ImageType[] ImageTypes { get; set; }
        public string[] SortBy { get; set; }
        public string[] MediaTypes { get; set; }
        public int? ImageTypeLimit { get; set; }
        public string[] IncludeItemTypes { get; set; }
        public ItemFilter[] Filters { get; set; }
        public ItemFields[] Fields { get; set; }
        public string ParentId { get; set; }
        public SortOrder? SortOrder { get; set; }
        public bool Recursive { get; set; }
        public int? Limit { get; set; }
        public int? StartIndex { get; set; }
        public string UserId { get; set; }
        public string[] ExcludeItemTypes { get; set; }
        public ImageType[] EnableImageTypes { get; set; }
    }
}
