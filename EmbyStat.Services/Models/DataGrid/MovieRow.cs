using System;

namespace EmbyStat.Services.Models.DataGrid
{
    public class MovieRow
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Container { get; set; }
        public string[] Subtitles { get; set; }
        public string[] AudioLanguages { get; set; }
        public string IMDB { get; set; }
        public int? TMDB { get; set; }
        public string TVDB { get; set; }
        public decimal RunTime { get; set; }
        public string OfficialRating { get; set; }
        public float? CommunityRating { get; set; }
        public string[] Genres { get; set; }
        public string Banner { get; set; }
        public string Logo { get; set; }
        public string Primary { get; set; }
        public string Thumb { get; set; }
        public string SortName { get; set; }
        public string Path { get; set; }
        public DateTimeOffset? PremiereDate { get; set; }
        public double SizeInMb { get; set; }
        public double BitRate { get; set; }
        public int? Height { get; set; }
        public int? Width { get; set; }
        public string Codec { get; set; }
        public int? BitDepth { get; set; }
        public string VideoRange { get; set; }
    }
}
