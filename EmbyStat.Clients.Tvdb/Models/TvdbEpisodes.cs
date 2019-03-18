using System.Collections.Generic;

namespace EmbyStat.Clients.Tvdb.Models
{
    public class TvdbEpisodes
    {
        public Links Links { get; set; }
        public List<Data> Data { get; set; }
        public Errors Errors { get; set; }
    }

    public class Links
    {
        public int First { get; set; }
        public int Last { get; set; }
        public int? Next { get; set; }
        public int? Prev { get; set; }
    }

    public class Language
    {
        public string EpisodeName { get; set; }
        public string Overview { get; set; }
    }

    public class Data
    {
        public int? AbsoluteNumber { get; set; }
        public int AiredEpisodeNumber { get; set; }
        public int AiredSeason { get; set; }
        public int AiredSeasonId { get; set; }
        public float? DvdEpisodeNumber { get; set; }
        public float? DvdSeason { get; set; }
        public string EpisodeName { get; set; }
        public string FirstAired { get; set; }
        public int Id { get; set; }
        public Language Language { get; set; }
        public int LastUpdated { get; set; }
        public string Overview { get; set; }
    }

    public class Errors
    {
        public string InvalidLanguage { get; set; }
    }
}
