using System;
using System.Collections.Generic;
using EmbyStat.Controllers.HelperClasses.Streams;

namespace EmbyStat.Controllers.Movie
{
    public class MovieRowViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Container { get; set; }
        public string[] Subtitles { get; set; }
        public string[] AudioLanguages { get; set; }
        public string IMDB { get; set; }
        public string TMDB { get; set; }
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
        public List<VideoStreamViewModel> VideoStreams { get; set; }
    }
}
