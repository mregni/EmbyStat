namespace EmbyStat.Controllers.Show
{
    public class ShowRowViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SortName { get; set; }
        public string Tvdb { get; set; }
        public string Imdb { get; set; }
        public int SeasonCount { get; set; }
        public int EpisodeCount { get; set; }
        public int SpecialEpisodeCount { get; set; }
        public int MissingEpisodeCount { get; set; }
        public string Status { get; set; }
        public long? RunTime { get; set; }
        public long? CumulativeRunTimeTicks { get; set; }
        public string[] Genres { get; set; }
        public string OfficialRating { get; set; }
        public double SizeInMb { get; set; }
    }
}
