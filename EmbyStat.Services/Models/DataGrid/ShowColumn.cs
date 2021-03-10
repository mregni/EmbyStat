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
        public int Seasons { get; set; }
        public int Episodes { get; set; }
        public int EpisodesData { get; set; }
        public int SizeInMb { get; set; }
        public int MissingEpisodes { get; set; }
    }
}
