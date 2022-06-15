namespace EmbyStat.Common.Models.Entities.Helpers;

public abstract class Extra : Media
{
    public decimal? CommunityRating { get; set; }
    public string IMDB { get; set; }
    public int? TMDB { get; set; }
    public int? TVDB { get; set; }
    public long? RunTimeTicks { get; set; }
    public string OfficialRating { get; set; }
}