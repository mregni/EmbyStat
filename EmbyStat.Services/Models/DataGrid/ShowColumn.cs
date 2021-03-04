using System;

namespace EmbyStat.Services.Models.DataGrid
{
    public class ShowColumn
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Container { get; set; }
        public string[] Subtitles { get; set; }
        public string[] AudioLanguages { get; set; }
        public string IMDB { get; set; }
        public string TMDB { get; set; }
        public string TVDB { get; set; }
        public float? CommunityRating { get; set; }
        public string SortName { get; set; }
        public string Path { get; set; }
        public double SizeInMb { get; set; }
        public string[] Seasons { get; set; }
        public string[] Episodes { get; set; }
    }
}
