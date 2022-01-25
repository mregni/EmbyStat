using System;
using System.Collections.Generic;
using EmbyStat.Common.SqLite;

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
        public List<SqlGenre> Genres { get; set; }
        public string Banner { get; set; }
        public string Logo { get; set; }
        public string Primary { get; set; }
        public string Thumb { get; set; }
        public string SortName { get; set; }
        public string Path { get; set; }
        public DateTimeOffset? PremiereDate { get; set; }
        public double SizeInMb { get; set; }
        public List<SqlVideoStream> VideoStreams { get; set; }
    }
}
