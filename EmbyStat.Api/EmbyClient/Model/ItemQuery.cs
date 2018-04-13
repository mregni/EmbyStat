using System;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Querying;

namespace EmbyStat.Api.EmbyClient.Model
{
    public class ItemQuery
    {
        public string UserId { get; set; }
        public string ParentId { get; set; }
        public int? StartIndex { get; set; }
        public int? Limit { get; set; }
        public string[] SortBy { get; set; }
        public string[] ArtistIds { get; set; }
        public SortOrder? SortOrder { get; set; }
        public ItemFilter[] Filters { get; set; }
        public ItemFields[] Fields { get; set; }
        public string[] MediaTypes { get; set; }
        public bool? Is3D { get; set; }
        public VideoType[] VideoTypes { get; set; }
        public bool Recursive { get; set; }
        public string[] Genres { get; set; }
        public string[] StudioIds { get; set; }
        public string[] ExcludeItemTypes { get; set; }
        public string[] IncludeItemTypes { get; set; }
        public int[] Years { get; set; }
        public string[] PersonIds { get; set; }
        public string[] PersonTypes { get; set; }
        public string SearchTerm { get; set; }
        public ImageType[] ImageTypes { get; set; }
        public DayOfWeek[] AirDays { get; set; }
        public SeriesStatus[] SeriesStatuses { get; set; }
        public string[] Ids { get; set; }
        public string MinOfficialRating { get; set; }
        public string MaxOfficialRating { get; set; }
        public int? MinIndexNumber { get; set; }
        public bool? HasParentalRating { get; set; }
        public bool? IsHD { get; set; }
        public int? ParentIndexNumber { get; set; }
        public int? MinPlayers { get; set; }
        public int? MaxPlayers { get; set; }
        public string NameStartsWithOrGreater { get; set; }
        public string NameStartsWith { get; set; }
        public string NameLessThan { get; set; }
        public string AlbumArtistStartsWithOrGreater { get; set; }
        public bool IncludeIndexContainers { get; set; }
        public LocationType[] LocationTypes { get; set; }
        public bool? IsMissing { get; set; }
        public bool? IsUnaired { get; set; }
        public bool? IsVirtualUnaired { get; set; }
        public bool? IsInBoxSet { get; set; }
        public bool? CollapseBoxSetItems { get; set; }
        public bool? IsPlayed { get; set; }
        public LocationType[] ExcludeLocationTypes { get; set; }
        public double? MinCommunityRating { get; set; }
        public double? MinCriticRating { get; set; }
        public int? AiredDuringSeason { get; set; }
        public DateTime? MinPremiereDate { get; set; }
        public DateTime? MaxPremiereDate { get; set; }
        public bool? EnableImages { get; set; }
        public int? ImageTypeLimit { get; set; }
        public ImageType[] EnableImageTypes { get; set; }
        public bool EnableTotalRecordCount { get; set; }

        public ItemQuery()
        {
            LocationTypes = new LocationType[] { };
            ExcludeLocationTypes = new LocationType[] { };
            SortBy = new string[] { };
            Filters = new ItemFilter[] { };
            Fields = new ItemFields[] { };
            MediaTypes = new string[] { };
            VideoTypes = new VideoType[] { };
            EnableTotalRecordCount = true;
            Genres = new string[] { };
            StudioIds = new string[] { };
            IncludeItemTypes = new string[] { };
            ExcludeItemTypes = new string[] { };
            Years = new int[] { };
            PersonTypes = new string[] { };
            Ids = new string[] { };
            ArtistIds = new string[] { };
            PersonIds = new string[] { };
            ImageTypes = new ImageType[] { };
            AirDays = new DayOfWeek[] { };
            SeriesStatuses = new SeriesStatus[] { };
            EnableImageTypes = new ImageType[] { };
        }
    }
}