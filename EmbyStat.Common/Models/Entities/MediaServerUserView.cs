using System;
using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Entities.Users;

namespace EmbyStat.Common.Models.Entities;

public class MediaServerUserView
{
    public string UserId { get; set; }
    public MediaServerUser User { get; set; }
    public string MediaType { get; set; }
    public string MediaId { get; set; }
    public int PlayCount { get; set; }
    public DateTime LastPlayedDate { get; set; }
}