using System.Collections.Generic;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Models.MediaServerUser;

public class MediaServerUserStatistics
{
    public List<Card<string>> Cards { get; set; }
}