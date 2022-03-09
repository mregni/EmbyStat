namespace EmbyStat.Common.SqLite.Helpers
{
    public abstract class SqlExtra : SqlMedia
    {
        public decimal? CommunityRating { get; set; }
        public string IMDB { get; set; }
        public int? TMDB { get; set; }
        public string TVDB { get; set; }
        public long? RunTimeTicks { get; set; }
        public string OfficialRating { get; set; }
    }
}
